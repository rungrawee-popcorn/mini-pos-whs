using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using MiniPOSWHS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniPOSWHS.Tests.SIT;

public class SaleFlowSITTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    // =========================
    // SEED FULL SYSTEM
    // =========================
    private async Task SeedSystem(AppDbContext context)
    {
        // User
        context.Users.Add(new User
        {
            UserId = 1,
            Username = "admin",
            PasswordHash = "hashed",
            Role = "Admin"
        });

        // Product
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
    // SIT: FULL SALE FLOW
    // =========================
    [Fact]
    public async Task SIT_FullSaleFlow_ShouldWorkEndToEnd()
    {
        var context = GetDbContext();
        await SeedSystem(context);

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

        // =========================
        // ACT
        // =========================
        var saleId = await saleService.CreateSaleAsync(cart, 1);

        // =========================
        // ASSERT ALL SYSTEMS
        // =========================

        var product = context.Products.First();
        var sale = context.Sales.FirstOrDefault();
        var details = context.SaleDetails.ToList();
        var logs = context.StockTransactions.ToList();
        var user = context.Users.First();

        // Sale created
        Assert.NotNull(sale);
        Assert.Equal(saleId, sale.SaleId);

        // Stock deducted
        Assert.Equal(8, product.StockQty);

        // Sale details created
        Assert.Single(details);
        Assert.Equal(1000, sale.TotalAmount);

        // Stock log created
        Assert.Single(logs);

        // User exists (system integration check)
        Assert.Equal("admin", user.Username);
    }
}