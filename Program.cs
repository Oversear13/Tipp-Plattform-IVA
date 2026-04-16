using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using TippPlattform.Models;
using TippPlattform.Models.SeedData;
using TippPlattform.Services;

var builder = WebApplication.CreateBuilder(args);

// Erlaubt externen Zugriff über lokale IP (z. B. für MAUI-Emulator)
builder.WebHost.UseUrls("http://0.0.0.0:5181", "https://0.0.0.0:7278");

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy)); // Require login by default
});

// ✅ Authentication konfigurieren
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);       // Optional: Cookie läuft ab
        options.SlidingExpiration = true;                     // Optional: Verlängert Ablaufzeit bei Aktivität
        options.AccessDeniedPath = "/Home/Login";             // Optional: Bei fehlender Berechtigung
    });

builder.Services.AddAuthorization();

// Database und Services
builder.Services.AddDbContext<TippPlattformContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TippPlattformConnection")));

builder.Services.AddSingleton<FootballDataService>();
builder.Services.AddScoped<TippPlattformService>();

// App bauen
var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Authentifizierung und Autorisierung aktivieren
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
