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

    // Display product list
    public async Task<IActionResult> Index(string keyword)
    {
        var query = _context.Products.AsQueryable();

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

    // Display create form
    public IActionResult Create()
    {
        return View();
    }

    // Handle create product
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product model)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.ProductCode = model.ProductCode.Trim();

        // CHECK DUPLICATE PRODUCT CODE
        var isDuplicate = await _context.Products
            .AnyAsync(x => x.ProductCode == model.ProductCode);

        if (isDuplicate)
        {
            ModelState.AddModelError("ProductCode", "Product Code already exists");
            return View(model);
        }

        model.CreatedDate = DateTime.Now;

        _context.Products.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Display edit form
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // Handle edit product
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var product = await _context.Products.FindAsync(model.ProductId);

        if (product == null)
            return NotFound();

        model.ProductCode = model.ProductCode.Trim();

        // CHECK DUPLICATE (exclude current record)
        var isDuplicate = await _context.Products
            .AnyAsync(x =>
                x.ProductCode == model.ProductCode &&
                x.ProductId != model.ProductId);

        if (isDuplicate)
        {
            ModelState.AddModelError("ProductCode", "Product Code already exists");
            return View(model);
        }

        product.ProductCode = model.ProductCode;
        product.ProductName = model.ProductName;
        product.Price = model.Price;
        product.StockQty = model.StockQty;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Delete product
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}