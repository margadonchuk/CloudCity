using System;
using System.Linq;
using System.Globalization;
using CloudCityCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Read environment before configuring EF
var env = builder.Environment;

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
        // Use InMemory database if no connection string is provided (for both Development and Production)
        opt.UseInMemoryDatabase("CloudCity");
    }
});

builder.Services
    .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()                // добавить
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure forwarded headers for reverse proxy (nginx)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

if (args.Any(a => a == "--seed" || a.StartsWith("--seed-admin=")))
{
    // Ensure connection string is provided via configuration/environment
    var cs = app.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(cs))
        throw new InvalidOperationException("ConnectionStrings__DefaultConnection is not set.");

    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    if (context.Database.IsRelational())
        await context.Database.MigrateAsync();

    var adminArg = args.FirstOrDefault(a => a.StartsWith("--seed-admin="));
    var adminEmail = adminArg?.Split('=', 2)[1];

    await SeedData.RunAsync(serviceProvider, adminEmail);

    if (args.Contains("--seed") && !context.Products.Any())
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
// Use forwarded headers FIRST before other middleware (for nginx reverse proxy)
// This must be called before UseHttpsRedirection
app.UseForwardedHeaders();

// Check if we're behind a reverse proxy (nginx)
var useReverseProxy = app.Configuration.GetValue<bool>("UseReverseProxy", false) || 
                      Environment.GetEnvironmentVariable("USE_REVERSE_PROXY") == "true";

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Don't use HSTS when behind reverse proxy - nginx handles HTTPS
    if (!useReverseProxy)
    {
        app.UseHsts();
    }
}
else
{
    app.UseDeveloperExceptionPage();
}

// Completely disable HTTPS redirection when behind reverse proxy (nginx handles HTTPS)
// This prevents ERR_TOO_MANY_REDIRECTS errors
if (!useReverseProxy)
{
    app.UseHttpsRedirection();
}

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
