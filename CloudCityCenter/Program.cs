using System;
using System.Linq;
using System.Globalization;
using CloudCityCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// ✅ Чтение строки подключения из переменной окружения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews().AddViewLocalization();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        if (connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase))
            opt.UseSqlServer(connectionString);
        else
            opt.UseSqlite(connectionString);
    }
    else
    {
        opt.UseInMemoryDatabase("CloudCity");
    }
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (args.Any(a => a == "--seed" || a.StartsWith("--seed-admin=")))
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsRelational())
        await context.Database.MigrateAsync();
    else
        await context.Database.EnsureCreatedAsync();

    var adminArg = args.FirstOrDefault(a => a.StartsWith("--seed-admin="));
    var adminEmail = adminArg?.Split('=', 2)[1];

    await SeedData.RunAsync(serviceProvider, adminEmail);

    if (args.Contains("--seed"))
    {
        SeedData.Initialize(context);
    }
    return;
}

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsRelational())
            context.Database.Migrate();
        else
            context.Database.EnsureCreated();
        if (!context.Products.Any())
        {
            SeedData.Initialize(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRequestLocalization(localizationOptions);
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

public partial class Program { }
