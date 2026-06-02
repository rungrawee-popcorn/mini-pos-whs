using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using MiniPOSWHS.Services;
using System;
using System.Threading.Tasks;

namespace MiniPOSWHS.Tests.AuthTests;

public class AuthServiceTests
{
    // =========================
    // CREATE IN-MEMORY DB
    // =========================
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        if (!context.Users.AnyAsync().Result)
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Role = "Admin",
                CreatedDate = DateTime.UtcNow
            });

            context.SaveChanges();
        }

        return context;
    }

    // =========================
    // TEST 1: LOGIN SUCCESS
    // =========================
    [Fact]
    public async Task Login_ShouldReturnTrue_WhenPasswordCorrect()
    {
        var context = GetDbContext();
        var service = new AuthService(context);

        var result = await service.Login("admin", "1234");

        Assert.True(result);
    }

    // =========================
    // TEST 2: USER NOT FOUND
    // =========================
    [Fact]
    public async Task Login_ShouldReturnFalse_WhenUserNotFound()
    {
        var context = GetDbContext();
        var service = new AuthService(context);

        var result = await service.Login("ghost", "1234");

        Assert.False(result);
    }

    // =========================
    // TEST 3: WRONG PASSWORD
    // =========================
    [Fact]
    public async Task Login_ShouldReturnFalse_WhenPasswordWrong()
    {
        var context = GetDbContext();
        var service = new AuthService(context);

        var result = await service.Login("admin", "wrongpass");

        Assert.False(result);
    }

    // =========================
    // TEST 4: GET USER
    // =========================
    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenExists()
    {
        var context = GetDbContext();
        var service = new AuthService(context);

        var result = await service.GetUser("admin");

        Assert.NotNull(result);
        Assert.Equal("admin", result.Username);
    }
}