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

public class StockBehaviorTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

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
    // TEST 1: STOCK DECREASE AFTER SALE
    // =========================
    [Fact]
    public async Task Stock_ShouldDecrease_AfterSale()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var saleService = new SaleService(context);

        var cart = new List<CartItem>
        {
            new CartItem
            {
                ProductId = 1,
                Qty = 3,
                Price = 500
            }
        };

        await saleService.CreateSaleAsync(cart, 1);

        var product = context.Products.First();

        Assert.Equal(7, product.StockQty);
    }

    // =========================
    // TEST 2: STOCK NEVER NEGATIVE
    // =========================
    [Fact]
    public async Task Stock_ShouldNotBeNegative()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var saleService = new SaleService(context);

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
            saleService.CreateSaleAsync(cart, 1)
        );

        var product = context.Products.First();

        Assert.True(product.StockQty >= 0);
    }

    // =========================
    // TEST 3: STOCK LOG CREATED
    // =========================
    [Fact]
    public async Task StockTransaction_ShouldBeCreated_WhenSaleSuccess()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var saleService = new SaleService(context);

        var cart = new List<CartItem>
        {
            new CartItem
            {
                ProductId = 1,
                Qty = 2,
                Price = 500
            }
        };

        await saleService.CreateSaleAsync(cart, 1);

        var logCount = context.StockTransactions.Count();

        Assert.Equal(1, logCount);
    }
}