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

    // Display dashboard
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var model = new DashboardViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),

            TotalSalesToday = await _context.Sales
                .Where(x => x.SaleDate.Date == today)
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,

            TotalTransactions = await _context.Sales.CountAsync(),

            LowStockProducts = await _context.Products
                .CountAsync(x => x.StockQty <= 10)
        };

        // =========================
        // Daily Sales 
        // =========================
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

        // =========================
        // Top Selling Products 
        // =========================
        model.TopSellingProducts = await _context.SaleDetails
            .Join(
                _context.Products,
                saleDetail => saleDetail.ProductId,
                product => product.ProductId,
                (saleDetail, product) => new
                {
                    product.ProductCode,
                    product.ProductName,
                    saleDetail.Qty
                })
            .GroupBy(x => new
            {
                x.ProductCode,
                x.ProductName
            })
            .Select(x => new TopSellingProductItem
            {
                ProductCode = x.Key.ProductCode,
                ProductName = x.Key.ProductName,
                TotalQty = x.Sum(y => y.Qty)
            })
            .OrderByDescending(x => x.TotalQty)
            .ThenBy(x => x.ProductCode)
            .Take(5)
            .ToListAsync();

        // =========================
        // Low Stock Alert 
        // =========================
        model.LowStockItems = await _context.Products
            .Where(x => x.StockQty <= 10)
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

    // Display privacy page
    public IActionResult Privacy()
    {
        return View();
    }

    // Handle application errors
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}