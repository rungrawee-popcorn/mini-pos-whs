using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.ViewModels;

namespace MiniPOSWHS.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly AppDbContext _context;

    public StockController(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // STOCK TRANSACTION HISTORY
    // =========================
    public async Task<IActionResult> Index()
    {
        var transactions = await (
            from st in _context.StockTransactions
            join p in _context.Products
                on st.ProductId equals p.ProductId
            orderby st.CreatedDate descending
            select new StockTransactionViewModel
            {
                TransactionId = st.TransactionId,
                ProductCode = p.ProductCode,
                ProductName = p.ProductName,
                TransactionType = st.TransactionType,
                Qty = st.Qty,
                CreatedDate = st.CreatedDate
            }
        ).ToListAsync();

        return View(transactions);
    }
}