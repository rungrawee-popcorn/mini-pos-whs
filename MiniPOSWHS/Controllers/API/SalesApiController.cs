using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class SalesApiController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesApiController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // GET: api/sales
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetSales()
    {
        var sales = await _context.Sales
            .OrderByDescending(x => x.SaleId)
            .Select(x => new
            {
                x.SaleId,
                x.SaleNo,
                x.SaleDate,
                x.TotalAmount,
                x.UserId
            })
            .ToListAsync();

        return Ok(sales);
    }

    // =========================
    // GET: api/sales/{id}
    // =========================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSaleDetail(int id)
    {
        var sale = await _context.Sales
            .Where(x => x.SaleId == id)
            .Select(x => new
            {
                x.SaleId,
                x.SaleNo,
                x.SaleDate,
                x.TotalAmount,
                Items = _context.SaleDetails
                    .Where(d => d.SaleId == x.SaleId)
                    .Join(_context.Products,
                        d => d.ProductId,
                        p => p.ProductId,
                        (d, p) => new
                        {
                            p.ProductCode,
                            p.ProductName,
                            d.Qty,
                            d.Price,
                            d.Amount
                        })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (sale == null)
            return NotFound();

        return Ok(sale);
    }

    // =========================
    // GET: api/sales/today
    // =========================
    [HttpGet("today")]
    public async Task<IActionResult> GetTodaySales()
    {
        var today = DateTime.Today;

        var total = await _context.Sales
            .Where(x => x.SaleDate.Date == today)
            .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

        var count = await _context.Sales
            .CountAsync(x => x.SaleDate.Date == today);

        return Ok(new
        {
            Date = today,
            TotalSales = total,
            TotalTransactions = count
        });
    }
}