using MiniPOSWHS.Data;
using MiniPOSWHS.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniPOSWHS.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Login(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username);

        if (user == null)
            return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<User?> GetUser(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username);
    }
}