


using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyHigh.Data;
using SkyHigh.Models;
using QuestPDF; // <-- Add this using

var builder = WebApplication.CreateBuilder(args);

// Accept QuestPDF license at app startup (REQUIRED for QuestPDF 2023.10+)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Use SQL Server (or UseSqlite if you want SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session
builder.Services.AddSession();

// Add cookie authentication for [Authorize] attributes to work
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();   // <-- Required for [Authorize]
app.UseAuthorization();    // <-- Required for [Authorize]

// Ensure the database is created and seeded with admin
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        db.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@skyairlines.com",
            PasswordHash = SkyHigh.Models.User.HashPassword("Admin@123"),
            DOB = new DateTime(1980, 1, 1),
            Phone = "9999999999",
            Role = "Admin"
        });
        db.SaveChanges();
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();