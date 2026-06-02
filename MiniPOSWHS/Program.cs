using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MiniPOSWHS.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniPOSWHS.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Database Context
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// Services
// =========================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SaleService>();

// =========================
// Authentication (Cookie)
// =========================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// =========================
// Session
// =========================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =========================
// MVC
// =========================
builder.Services.AddControllersWithViews();

// =========================
// Culture
// =========================
var culture = new CultureInfo("en-US");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var app = builder.Build();

// =========================
// Middleware Pipeline
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANT ORDER
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// Routes
// =========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();