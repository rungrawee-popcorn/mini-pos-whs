using MiniPOSWHS.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace MiniPOSWHS.Data;

public static class DbSeeder
{
    // =========================
    // FIX ADMIN LOGIN ISSUE
    // =========================
    public static async Task SeedAdmin(AppDbContext context)
    {
        // Ensure DB is ready
        await context.Database.EnsureCreatedAsync();

        var admin = await context.Users
            .FirstOrDefaultAsync(x => x.Username == "admin");

        if (admin == null)
        {
            // CREATE NEW ADMIN
            admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                Role = "Admin",
                CreatedDate = DateTime.UtcNow
            };

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
        else
        {
            // FIX OLD INVALID HASH (IMPORTANT)
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234");
            await context.SaveChangesAsync();
        }
    }
}