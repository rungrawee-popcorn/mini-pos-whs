using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Models;

namespace MiniPOSWHS.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<SaleDetail> SaleDetails { get; set; }

    public DbSet<StockTransaction> StockTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Sale>()
            .Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SaleDetail>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<SaleDetail>()
            .Property(x => x.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Role = "Admin",
                CreatedDate = DateTime.Now
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductId = 1,
                ProductCode = "P001",
                ProductName = "Coke",
                Price = 15,
                StockQty = 100,
                CreatedDate = DateTime.Now
            },
            new Product
            {
                ProductId = 2,
                ProductCode = "P002",
                ProductName = "Water",
                Price = 10,
                StockQty = 100,
                CreatedDate = DateTime.Now
            },
            new Product
            {
                ProductId = 3,
                ProductCode = "P003",
                ProductName = "Snack",
                Price = 20,
                StockQty = 100,
                CreatedDate = DateTime.Now
            }
        );
    }
}