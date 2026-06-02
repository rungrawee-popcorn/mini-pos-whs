using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;

namespace MiniPOSWHS.Controllers.API;

[Route("api/dashboard")]
[ApiController]
public class DashboardApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardApiController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var today = DateTime.Today;

        var totalProducts = await _context.Products.CountAsync();

        var totalSalesToday = await _context.Sales
            .Where(x => x.SaleDate.Date == today)
            .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

        var totalTransactions = await _context.Sales.CountAsync();

        var lowStockProducts = await _context.Products
            .CountAsync(x => x.StockQty <= 10);

        var topSellingProducts = await _context.SaleDetails
            .Join(_context.Products,
                sd => sd.ProductId,
                p => p.ProductId,
                (sd, p) => new
                {
                    p.ProductCode,
                    p.ProductName,
                    sd.Qty
                })
            .GroupBy(x => new { x.ProductCode, x.ProductName })
            .Select(x => new
            {
                x.Key.ProductCode,
                x.Key.ProductName,
                TotalQty = x.Sum(y => y.Qty)
            })
            .OrderByDescending(x => x.TotalQty)
            .Take(5)
            .ToListAsync();

        var dailySales = await _context.Sales
            .GroupBy(x => x.SaleDate.Date)
            .Select(x => new
            {
                SaleDate = x.Key,
                TotalAmount = x.Sum(y => y.TotalAmount)
            })
            .OrderByDescending(x => x.SaleDate)
            .Take(7)
            .ToListAsync();

        return Ok(new
        {
            totalProducts,
            totalSalesToday,
            totalTransactions,
            lowStockProducts,
            topSellingProducts,
            dailySales
        });
    }
}