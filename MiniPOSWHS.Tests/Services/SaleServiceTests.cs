using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using MiniPOSWHS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniPOSWHS.Tests.Services;

public class SaleServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // =========================
    // SEED PRODUCT
    // =========================
    private async Task SeedProduct(AppDbContext context)
    {
        context.Products.Add(new Product
        {
            ProductId = 1,
            ProductCode = "P001",
            ProductName = "Keyboard",
            Price = 500,
            StockQty = 10,
            CreatedDate = DateTime.Now
        });

        await context.SaveChangesAsync();
    }

    // =========================
    // TEST 1: SUCCESS SALE
    // =========================
    [Fact]
    public async Task CreateSaleAsync_ShouldSuccess_AndDeductStock()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new SaleService(context);

        var cart = new List<CartItem>
        {
            new CartItem
            {
                ProductId = 1,
                Qty = 2,
                Price = 500
            }
        };

        var saleId = await service.CreateSaleAsync(cart, 1);

        var product = context.Products.First();
        var sale = context.Sales.FirstOrDefault();
        var details = context.SaleDetails.ToList();
        var logs = context.StockTransactions.ToList();

        Assert.Equal(8, product.StockQty);              // stock ลด
        Assert.NotNull(sale);                          // sale header
        Assert.Single(details);                        // 1 detail
        Assert.Single(logs);                           // 1 stock log
        Assert.True(saleId > 0);
    }

    // =========================
    // TEST 2: INSUFFICIENT STOCK
    // =========================
    [Fact]
    public async Task CreateSaleAsync_ShouldFail_WhenStockNotEnough()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new SaleService(context);

        var cart = new List<CartItem>
        {
            new CartItem
            {
                ProductId = 1,
                Qty = 999,
                Price = 500
            }
        };

        await Assert.ThrowsAsync<Exception>(() =>
            service.CreateSaleAsync(cart, 1)
        );

        // ต้องไม่เกิด sale
        Assert.Empty(context.Sales);
        Assert.Empty(context.SaleDetails);
        Assert.Empty(context.StockTransactions);
    }
}