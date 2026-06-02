using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Services;

public class SaleService
{
    private readonly AppDbContext _context;

    public SaleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateSaleAsync(List<CartItem> cart, int userId)
    {
        if (cart == null || !cart.Any())
            throw new Exception("Cart is empty");

        // =========================
        // FIX: support InMemory DB
        // =========================
        var isInMemory =
            _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";

        IDbContextTransaction? transaction = null;

        if (!isInMemory)
        {
            transaction = await _context.Database.BeginTransactionAsync();
        }

        try
        {
            // =========================
            // 1. VALIDATE STOCK
            // =========================
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product not found: {item.ProductId}");

                if (product.StockQty < item.Qty)
                    throw new Exception($"Insufficient stock: {product.ProductCode}");
            }

            // =========================
            // 2. CREATE SALE HEADER
            // =========================
            var sale = new Sale
            {
                SaleNo = $"S-{DateTime.Now:yyyyMMddHHmmss}",
                SaleDate = DateTime.Now,
                TotalAmount = cart.Sum(x => x.Price * x.Qty),
                UserId = userId
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // =========================
            // 3. DETAILS + STOCK + LOG
            // =========================
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product not found: {item.ProductId}");

                var detail = new SaleDetail
                {
                    SaleId = sale.SaleId,
                    ProductId = item.ProductId,
                    Qty = item.Qty,
                    Price = item.Price,
                    Amount = item.Price * item.Qty
                };

                _context.SaleDetails.Add(detail);

                // Deduct Stock
                product.StockQty -= item.Qty;

                // Stock Log
                var stockLog = new StockTransaction
                {
                    ProductId = item.ProductId,
                    UserId = userId,
                    TransactionType = "OUT",
                    Qty = item.Qty,
                    CreatedDate = DateTime.Now
                };

                _context.StockTransactions.Add(stockLog);
            }

            await _context.SaveChangesAsync();

            // =========================
            // COMMIT
            // =========================
            if (transaction != null)
                await transaction.CommitAsync();

            return sale.SaleId;
        }
        catch (Exception)
        {
            // =========================
            // ROLLBACK
            // =========================
            if (transaction != null)
                await transaction.RollbackAsync();

            throw;
        }
    }
}