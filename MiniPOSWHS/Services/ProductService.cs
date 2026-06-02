using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniPOSWHS.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // =========================
    // CREATE PRODUCT
    // =========================
    public async Task CreateProduct(Product product)
    {
        var exists = await _context.Products
            .AnyAsync(x => x.ProductCode == product.ProductCode);

        if (exists)
            throw new Exception("Duplicate product code");

        product.CreatedDate = DateTime.UtcNow;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    // =========================
    // GET BY CODE
    // =========================
    public async Task<Product?> GetProductByCode(string code)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x => x.ProductCode == code);
    }

    // =========================
    // UPDATE PRODUCT
    // =========================
    public async Task UpdateProduct(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    // =========================
    // DECREASE STOCK
    // =========================
    public async Task DecreaseStock(string code, int qty)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductCode == code);

        if (product == null)
            throw new Exception("Product not found");

        if (product.StockQty < qty)
            throw new Exception("Stock not enough");

        product.StockQty -= qty;

        await _context.SaveChangesAsync();
    }
}