using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniPOSWHS.Services;

namespace MiniPOSWHS.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // Display login page
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // Handle login request
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var isValid = await _authService.Login(username, password);

        if (!isValid)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        var user = await _authService.GetUser(username);

        if (user == null)
        {
            ViewBag.Error = "User not found";
            return View();
        }

        // Create user claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Sign in user using cookie authentication
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return RedirectToAction("Index", "Home");
    }

    // Logout user and clear authentication cookie
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}