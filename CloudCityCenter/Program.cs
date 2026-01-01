using System;
using System.Linq;
using System.Globalization;
using CloudCityCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку systemd для Type=notify (если на Linux)
if (System.OperatingSystem.IsLinux())
{
    builder.Host.UseSystemd();
}

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

// Добавляем глобальный обработчик необработанных исключений
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    var exception = e.ExceptionObject as Exception;
    Console.Error.WriteLine($"CRITICAL: Unhandled exception: {exception?.Message}");
    Console.Error.WriteLine($"Stack trace: {exception?.StackTrace}");
    if (exception?.InnerException != null)
    {
        Console.Error.WriteLine($"Inner exception: {exception.InnerException.Message}");
    }
};

// Обработчик необработанных исключений для async операций
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Console.Error.WriteLine($"CRITICAL: Unobserved task exception: {e.Exception.Message}");
    Console.Error.WriteLine($"Stack trace: {e.Exception.StackTrace}");
    e.SetObserved();
};

if (args.Any(a => a == "--seed" || a.StartsWith("--seed-admin=") || a == "--migrate-data"))
{
    // Ensure connection string is provided via configuration/environment
    var cs = app.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(cs))
    {
        Console.WriteLine("❌ ОШИБКА: ConnectionStrings__DefaultConnection не установлена.");
        Console.WriteLine("Установите строку подключения через:");
        Console.WriteLine("  - appsettings.Production.json");
        Console.WriteLine("  - Переменную окружения: export ConnectionStrings__DefaultConnection=\"...\"");
        throw new InvalidOperationException("ConnectionStrings__DefaultConnection is not set.");
    }

    Console.WriteLine($"✓ Строка подключения найдена: {cs.Substring(0, Math.Min(50, cs.Length))}...");

    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        Console.WriteLine("Проверка подключения к базе данных...");
        if (context.Database.IsRelational())
        {
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                Console.WriteLine("❌ Не удалось подключиться к базе данных!");
                Console.WriteLine("Проверьте:");
                Console.WriteLine("  1. Доступность SQL Server на 10.151.10.8");
                Console.WriteLine("  2. Правильность логина и пароля");
                Console.WriteLine("  3. Существование базы данных CloudCityDB");
                throw new InvalidOperationException("Cannot connect to database");
            }
            Console.WriteLine("✓ Подключение к базе данных успешно");

            Console.WriteLine("Применение миграций...");
            await context.Database.MigrateAsync();
            Console.WriteLine("✓ Миграции применены");
        }
        else
        {
            Console.WriteLine("⚠ Используется нереляционная база данных");
        }

        var adminArg = args.FirstOrDefault(a => a.StartsWith("--seed-admin="));
        var adminEmail = adminArg?.Split('=', 2)[1];

        Console.WriteLine("Создание ролей...");
        await SeedData.RunAsync(serviceProvider, adminEmail);
        Console.WriteLine("✓ Роли созданы");

        if (args.Contains("--seed") || args.Contains("--migrate-data"))
        {
            Console.WriteLine("Проверка существующих товаров...");
            var existingProductsCount = await context.Products.CountAsync();
            Console.WriteLine($"Найдено товаров в базе: {existingProductsCount}");

            if (existingProductsCount == 0)
            {
                Console.WriteLine("Загрузка товаров и услуг в базу данных...");
                SeedData.Initialize(context);
                var productsCount = await context.Products.CountAsync();
                var variantsCount = await context.ProductVariants.CountAsync();
                var featuresCount = await context.ProductFeatures.CountAsync();
                Console.WriteLine($"✓ Загрузка завершена:");
                Console.WriteLine($"  - Товаров: {productsCount}");
                Console.WriteLine($"  - Вариантов: {variantsCount}");
                Console.WriteLine($"  - Характеристик: {featuresCount}");
            }
            else
            {
                Console.WriteLine($"⚠ Товары уже загружены в базу данных ({existingProductsCount} товаров).");
                Console.WriteLine("Для перезагрузки удалите товары из базы данных.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ОШИБКА при миграции: {ex.Message}");
        Console.WriteLine($"Детали: {ex}");
        throw;
    }
    
    Console.WriteLine("\n✅ Миграция данных успешно завершена!");
    return;
}

var supportedCultures = new[] 
{ 
    new CultureInfo("en"),  // English
    new CultureInfo("uk"),  // Ukrainian
    new CultureInfo("ru"),  // Russian
    new CultureInfo("fr"),  // French
    new CultureInfo("de"),  // German
    new CultureInfo("es"),  // Spanish
    new CultureInfo("pl")   // Polish
};
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    }
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
            var providerName = context.Database.ProviderName;
            logger.LogInformation($"Database provider: {providerName}");
            
            // Проверяем, что это SQL Server, а не SQLite
            if (providerName != null && providerName.Contains("SqlServer"))
            {
                // Проверяем, существуют ли уже таблицы (пробуем выполнить простой запрос)
                bool productsTableExists = false;
                try
                {
                    await context.Products.CountAsync();
                    productsTableExists = true;
                }
                catch
                {
                    // Если не удалось, значит таблицы нет
                    productsTableExists = false;
                }
                
                if (!productsTableExists)
                {
                    logger.LogInformation("Tables not found. Attempting to apply migrations...");
                    try
                    {
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Migrations applied successfully");
                    }
                    catch (Exception migEx)
                    {
                        logger.LogWarning(migEx, "Could not apply migrations (migrations might be for SQLite).");
                        logger.LogWarning("Solution: Create tables manually using create_database_sqlserver.sql script in SQL Server Management Studio.");
                        logger.LogWarning("The application will continue, but database operations may fail until tables are created.");
                    }
                }
                else
                {
                    logger.LogInformation("Tables already exist. Skipping migrations.");
                }
            }
            else
            {
                logger.LogInformation("Applying migrations for non-SQL Server database...");
                try
                {
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");
                }
                catch
                {
                    // Игнорируем ошибки миграций для не-SQL Server баз
                }
            }
        }
        else
        {
            logger.LogInformation("Using non-relational database, ensuring created...");
            context.Database.EnsureCreated();
        }
        
        // Загружаем данные только если товаров нет
        var productsCount = await context.Products.CountAsync();
        if (productsCount == 0)
        {
            logger.LogInformation("Database is empty, seeding initial data...");
            SeedData.Initialize(context);
            productsCount = await context.Products.CountAsync();
            logger.LogInformation($"Initial data seeded successfully. Seeded {productsCount} products");
        }
        else
        {
            logger.LogInformation($"Database already contains {productsCount} products");
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
    app.UseHsts();
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
