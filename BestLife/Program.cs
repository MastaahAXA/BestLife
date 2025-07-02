using BestLife.Data;
using BestLife.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure SQL Server Database Context
builder.Services.AddDbContext<BestLifeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default_Connection")));

// ✅ Register IHttpContextAccessor for accessing user claims
builder.Services.AddHttpContextAccessor();

// ✅ Register Password Hasher
builder.Services.AddScoped<IPasswordHasher<Registration>, PasswordHasher<Registration>>(); // Fixes injection error

// ✅ Configure Cookie Authentication (AccessDeniedPath removed)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        // ❌ AccessDeniedPath removed
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// ✅ Configure Session Support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Error Handling and HSTS in Production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Middleware Pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // 1. Authenticate
app.UseSession();        // 2. Maintain session
app.UseAuthorization();  // 3. Enforce role-based access

// ✅ Optional: Admin Seeding
// await SeedDefaultAdmin(app.Services);

// ✅ Default Route: Go to Login if no route is provided
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
