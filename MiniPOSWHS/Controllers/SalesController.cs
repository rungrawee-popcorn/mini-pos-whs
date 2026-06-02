using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using MiniPOSWHS.Services;
using System.Text.Json;

namespace MiniPOSWHS.Controllers;

[Authorize]
public class SalesController : Controller
{
    private readonly AppDbContext _context;
    private readonly SaleService _saleService;

    private const string CART_KEY = "CART_SESSION";

    public SalesController(AppDbContext context, SaleService saleService)
    {
        _context = context;
        _saleService = saleService;
    }

    // =========================
    // POS SCREEN
    // =========================
    public async Task<IActionResult> POS(string keyword)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(x =>
                x.ProductCode.Contains(keyword) ||
                x.ProductName.Contains(keyword));
        }

        ViewBag.Keyword = keyword;
        ViewBag.Cart = GetCart();
        ViewBag.Total = GetCartTotal();

        var products = await query.ToListAsync();

        return View(products);
    }

    // =========================
    // ADD TO CART
    // =========================
    [HttpPost]
    public async Task<IActionResult> AddToCart(int productId)
    {
        var product = await _context.Products.FindAsync(productId);

        if (product == null)
            return NotFound();

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Qty += 1;

            if (existingItem.Qty < 1)
                existingItem.Qty = 1;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                Qty = 1
            });
        }

        SaveCart(cart);

        return RedirectToAction(nameof(POS));
    }

    // =========================
    // CHECKOUT
    // =========================
    [HttpPost]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();

        if (cart == null || !cart.Any())
            return RedirectToAction(nameof(POS));

        int userId = 1;

        try
        {
            await _saleService.CreateSaleAsync(cart, userId);

            HttpContext.Session.Remove(CART_KEY);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(POS));
        }

        return RedirectToAction(nameof(POS));
    }

    // =========================
    // CART HELPERS
    // =========================
    private List<CartItem> GetCart()
    {
        var session = HttpContext.Session.GetString(CART_KEY);

        if (string.IsNullOrEmpty(session))
            return new List<CartItem>();

        return JsonSerializer.Deserialize<List<CartItem>>(session)
               ?? new List<CartItem>();
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString(
            CART_KEY,
            JsonSerializer.Serialize(cart)
        );
    }

    private decimal GetCartTotal()
    {
        var cart = GetCart();
        return cart.Sum(x => x.Price * x.Qty);
    }
}