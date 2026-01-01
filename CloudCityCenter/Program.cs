using System;
using System.Linq;
using System.Globalization;
using CloudCityCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Read environment before configuring EF
var env = builder.Environment;

// ✅ Чтение строки подключения из переменной окружения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Логируем информацию о подключении (безопасно, без пароля)
if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("⚠️ WARNING: ConnectionStrings__DefaultConnection is not set! Will use InMemory database.");
    Console.WriteLine("⚠️ Set environment variable: export ConnectionStrings__DefaultConnection=\"Server=...;Database=...;User Id=...;Password=...\"");
}
else
{
    // Логируем connection string без пароля для безопасности
    var safeConnectionString = connectionString.Contains("Password=", StringComparison.OrdinalIgnoreCase)
        ? connectionString.Substring(0, connectionString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase)) + "Password=***"
        : connectionString;
    Console.WriteLine($"✅ Database connection string found: {safeConnectionString}");
}

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
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Логируем информацию о базе данных
        var dbProvider = context.Database.ProviderName ?? "Unknown";
        var isInMemory = context.Database.IsInMemory();
        var isRelational = context.Database.IsRelational();
        var canConnect = await context.Database.CanConnectAsync();
        
        logger.LogInformation("Database Provider: {Provider}", dbProvider);
        logger.LogInformation("Is InMemory: {IsInMemory}", isInMemory);
        logger.LogInformation("Is Relational: {IsRelational}", isRelational);
        logger.LogInformation("Can Connect: {CanConnect}", canConnect);
        
        if (isInMemory)
        {
            logger.LogWarning("⚠️ WARNING: Using InMemory database! Data will not persist. Check ConnectionStrings__DefaultConnection environment variable.");
        }
        
        if (context.Database.IsRelational())
        {
            logger.LogInformation("Applying migrations to relational database...");
            context.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("Using non-relational database, ensuring created...");
            context.Database.EnsureCreated();
        }
        
        if (!context.Products.Any())
        {
            logger.LogInformation("Database is empty, seeding initial data...");
            SeedData.Initialize(context);
            logger.LogInformation("Initial data seeded successfully.");
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
else
{
    app.UseDeveloperExceptionPage();
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

// Agent debug ingest endpoint (writes NDJSON lines to .cursor/debug.log)
var debugDir = Path.Combine(app.Environment.ContentRootPath, ".cursor");
var debugLogPath = Path.Combine(debugDir, "debug.log");
Directory.CreateDirectory(debugDir);
app.MapPost("/ingest/38680bd6-8223-4b5d-9453-913b38e9d421", async context =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    if (!string.IsNullOrWhiteSpace(body))
    {
        await File.AppendAllTextAsync(debugLogPath, body.TrimEnd() + Environment.NewLine);
    }
    context.Response.StatusCode = 200;
});

app.Run();

public partial class Program { }
