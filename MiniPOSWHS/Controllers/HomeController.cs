using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var model = new DashboardViewModel
        {
            TotalProducts = await _context.Products
                .Where(x => !x.IsDeleted)
                .CountAsync(),

            TotalSalesToday = await _context.Sales
                .Where(x => x.SaleDate.Date == today)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,

            TotalTransactions = await _context.Sales.CountAsync(),

            LowStockProducts = await _context.Products
                .Where(x => !x.IsDeleted)
                .CountAsync(x => x.StockQty <= 10)
        };

        model.DailySales = await _context.Sales
            .GroupBy(x => x.SaleDate.Date)
            .Select(x => new DailySalesItem
            {
                SaleDate = x.Key,
                TotalAmount = x.Sum(y => y.TotalAmount)
            })
            .OrderByDescending(x => x.SaleDate)
            .Take(7)
            .ToListAsync();

        model.TopSellingProducts = await _context.SaleDetails
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQty = g.Sum(x => x.Qty)
            })
            .Join(
                _context.Products,
                g => g.ProductId,
                p => p.ProductId,
                (g, p) => new TopSellingProductItem
                {
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    TotalQty = g.TotalQty
                }
            )
            .OrderByDescending(x => x.TotalQty)
            .Take(5)
            .ToListAsync();

        model.LowStockItems = await _context.Products
            .Where(x => !x.IsDeleted && x.StockQty <= 10)
            .OrderBy(x => x.StockQty)
            .Select(x => new LowStockProductItem
            {
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                StockQty = x.StockQty
            })
            .ToListAsync();

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Error()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}