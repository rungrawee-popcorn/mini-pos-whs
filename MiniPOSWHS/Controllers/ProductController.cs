using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly AppDbContext _context;

    public ProductController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // LIST
    // =========================
    public async Task<IActionResult> Index(string keyword)
    {
        var query = _context.Products
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>
                x.ProductCode.Contains(keyword) ||
                x.ProductName.Contains(keyword));
        }

        var products = await query
            .OrderByDescending(x => x.ProductId)
            .ToListAsync();

        ViewBag.Keyword = keyword;

        return View(products);
    }

    // =========================
    // CREATE
    // =========================
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.ProductCode = model.ProductCode.Trim();

        // Product Code must remain unique even if product was deleted
        var isDuplicate = await _context.Products
            .AnyAsync(x => x.ProductCode == model.ProductCode);

        if (isDuplicate)
        {
            ModelState.AddModelError(
                nameof(model.ProductCode),
                "Product Code already exists");

            return View(model);
        }

        model.CreatedDate = DateTime.Now;

        _context.Products.Add(model);
        await _context.SaveChangesAsync();

        // Create initial stock transaction
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            _context.StockTransactions.Add(new StockTransaction
            {
                ProductId = model.ProductId,
                UserId = int.Parse(userIdClaim.Value),
                TransactionType = "IN",
                Qty = model.StockQty,
                Source = "INITIAL_STOCK",
                CreatedDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // EDIT
    // =========================
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == id && !x.IsDeleted);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == model.ProductId && !x.IsDeleted);

        if (product == null)
            return NotFound();

        model.ProductCode = model.ProductCode.Trim();

        // Product Code must remain unique even if product was deleted
        var isDuplicate = await _context.Products.AnyAsync(x =>
            x.ProductCode == model.ProductCode &&
            x.ProductId != model.ProductId);

        if (isDuplicate)
        {
            ModelState.AddModelError(
                nameof(model.ProductCode),
                "Product Code already exists");

            return View(model);
        }

        var oldStock = product.StockQty;
        var newStock = model.StockQty;

        product.ProductCode = model.ProductCode;
        product.ProductName = model.ProductName;
        product.Price = model.Price;
        product.StockQty = newStock;

        var diff = newStock - oldStock;

        if (diff != 0)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                _context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.ProductId,
                    UserId = int.Parse(userIdClaim.Value),
                    TransactionType = diff > 0 ? "IN" : "OUT",
                    Qty = Math.Abs(diff),
                    Source = "STOCK_ADJUSTMENT",
                    CreatedDate = DateTime.Now
                });
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // DELETE (SOFT DELETE)
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == id);

        if (product == null)
            return NotFound();

        // Capture remaining stock before deletion
        var remainingStock = product.StockQty;

        // Soft delete
        product.IsDeleted = true;

        // Clear stock after removing product from active inventory
        product.StockQty = 0;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null)
        {
            _context.StockTransactions.Add(new StockTransaction
            {
                ProductId = product.ProductId,
                UserId = int.Parse(userIdClaim.Value),
                TransactionType = "OUT",
                Qty = remainingStock,
                Source = "PRODUCT_DELETED",
                CreatedDate = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}