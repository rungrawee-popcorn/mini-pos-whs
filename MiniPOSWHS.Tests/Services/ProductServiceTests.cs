using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using MiniPOSWHS.Services;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace MiniPOSWHS.Tests.Services;

public class ProductServiceTests
{
    // =========================
    // CREATE IN-MEMORY DB
    // =========================
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
            ProductCode = "P001",
            ProductName = "Keyboard",
            Price = 500,
            StockQty = 10,   // FIXED
            CreatedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }

    // =========================
    // TEST 1: CREATE PRODUCT
    // =========================
    [Fact]
    public async Task CreateProduct_ShouldAddProduct_WhenValid()
    {
        var context = GetDbContext();
        var service = new ProductService(context);

        var product = new Product
        {
            ProductCode = "P002",
            ProductName = "Mouse",
            Price = 300,
            StockQty = 5,   // FIXED
            CreatedDate = DateTime.UtcNow
        };

        await service.CreateProduct(product);

        var result = context.Products.FirstOrDefault(x => x.ProductCode == "P002");

        Assert.NotNull(result);
        Assert.Equal("Mouse", result.ProductName);
    }

    // =========================
    // TEST 2: DUPLICATE PRODUCT CODE
    // =========================
    [Fact]
    public async Task CreateProduct_ShouldFail_WhenDuplicateCode()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new ProductService(context);

        var product = new Product
        {
            ProductCode = "P001",
            ProductName = "Mouse",
            Price = 300,
            StockQty = 5,   // FIXED
            CreatedDate = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<Exception>(() => service.CreateProduct(product));
    }

    // =========================
    // TEST 3: GET PRODUCT BY CODE
    // =========================
    [Fact]
    public async Task GetProduct_ShouldReturnProduct_WhenExists()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new ProductService(context);

        var result = await service.GetProductByCode("P001");

        Assert.NotNull(result);
        Assert.Equal("Keyboard", result.ProductName);
    }

    // =========================
    // TEST 4: UPDATE PRODUCT
    // =========================
    [Fact]
    public async Task UpdateProduct_ShouldChangeData_WhenValid()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new ProductService(context);

        var product = context.Products.First();

        product.ProductName = "Mechanical Keyboard";
        product.Price = 1200;

        await service.UpdateProduct(product);

        var updated = context.Products.First();

        Assert.Equal("Mechanical Keyboard", updated.ProductName);
        Assert.Equal(1200, updated.Price);
    }

    // =========================
    // TEST 5: CHECK STOCK
    // =========================
    [Fact]
    public async Task Stock_ShouldDecrease_WhenSellProduct()
    {
        var context = GetDbContext();
        await SeedProduct(context);

        var service = new ProductService(context);

        await service.DecreaseStock("P001", 3);

        var product = context.Products.First();

        Assert.Equal(7, product.StockQty);  // FIXED
    }
}