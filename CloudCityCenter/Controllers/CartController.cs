using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizerFactory _localizerFactory;
    private const string CartSessionKey = "Cart";

    public CartController(ApplicationDbContext context, IStringLocalizerFactory localizerFactory)
    {
        _context = context;
        _localizerFactory = localizerFactory;
    }

    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.Cart.Index", "CloudCityCenter");
    }

    private List<OrderItem> GetCart()
    {
        var cartJson = HttpContext.Session.GetString(CartSessionKey);
        return cartJson != null
            ? JsonSerializer.Deserialize<List<OrderItem>>(cartJson) ?? new List<OrderItem>()
            : new List<OrderItem>();
    }

    private void SaveCart(List<OrderItem> cart)
    {
        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(cart));
    }

    public async Task<IActionResult> Index()
    {
        var cart = GetCart();
        var productIds = cart.Select(c => c.ProductId).ToList();
        var variantIds = cart.Where(c => c.ProductVariantId.HasValue).Select(c => c.ProductVariantId!.Value).ToList();

        var products = await _context.Products
            .AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        var variants = await _context.ProductVariants
            .AsNoTracking()
            .Where(v => variantIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id);

        var items = cart.Select(c => new CartItemViewModel
        {
            Item = c,
            Product = products.ContainsKey(c.ProductId) ? products[c.ProductId] : null,
            ProductVariant = c.ProductVariantId.HasValue && variants.ContainsKey(c.ProductVariantId.Value)
                ? variants[c.ProductVariantId.Value]
                : null
        }).ToList();

        var vm = new CartViewModel
        {
            Items = items,
            Total = cart.Sum(i => i.Price)
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int? productVariantId, bool includeSetupService = false, decimal? setupServicePrice = null)
    {
        try
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                if (IsAjaxRequest())
                {
                    return Json(new { success = false, message = "Product not found" });
                }
                return NotFound();
            }

            decimal price;
            if (productVariantId.HasValue)
            {
                var variant = await _context.ProductVariants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == productVariantId.Value && v.ProductId == productId);
                if (variant == null)
                {
                    if (IsAjaxRequest())
                    {
                        return Json(new { success = false, message = "Product variant not found" });
                    }
                    return BadRequest();
                }

                price = variant.Price;
            }
            else
            {
                price = product.PricePerMonth;
            }

            var cart = GetCart();
            int itemsAdded = 1;
            
            // Добавляем товар в корзину
            cart.Add(new OrderItem { ProductId = productId, ProductVariantId = productVariantId, Price = price });
            
            // Если выбрана услуга настройки, добавляем её тоже
            if (includeSetupService && setupServicePrice.HasValue && setupServicePrice.Value > 0)
            {
                var setupServiceName = GetLocalizer()["WindowsServerSetupService"].Value;
                var setupPrice = setupServicePrice.Value;
                
                // Используем существующий метод для добавления услуги настройки
                var setupSlug = $"windows-server-setup-{productId}";
                var setupProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Slug == setupSlug);

                if (setupProduct == null)
                {
                    setupProduct = new Product
                    {
                        Name = setupServiceName,
                        Slug = setupSlug,
                        Type = ProductType.VPN, // Используем VPN тип для услуг настройки
                        Location = product.Location,
                        PricePerMonth = setupPrice,
                        Configuration = $"Windows Server Setup & Installation service for {product.Name}",
                        IsAvailable = true,
                        IsPublished = false, // Не показываем в каталоге
                        ImageUrl = "/images/setupWinserv.png"
                    };
                    _context.Products.Add(setupProduct);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Обновляем изображение, если его нет
                    if (string.IsNullOrEmpty(setupProduct.ImageUrl))
                    {
                        setupProduct.ImageUrl = "/images/setupWinserv.png";
                        await _context.SaveChangesAsync();
                    }
                }

                cart.Add(new OrderItem { ProductId = setupProduct.Id, ProductVariantId = null, Price = setupPrice });
                itemsAdded++;
            }
            
            SaveCart(cart);

            // Проверяем, является ли это AJAX запросом
            if (IsAjaxRequest())
            {
                return Json(new { success = true, message = GetLocalizer()["ProductAddedToCart"].Value, itemsAdded = itemsAdded });
            }

            TempData["Success"] = GetLocalizer()["ProductAddedToCart"].Value;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Логируем ошибку (в production можно использовать ILogger)
            Console.Error.WriteLine($"Error adding product to cart: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");

            if (IsAjaxRequest())
            {
                return Json(new { success = false, message = GetLocalizer()["ErrorOccurred"].Value });
            }

            TempData["Error"] = GetLocalizer()["ErrorOccurred"].Value;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMaintenanceService(string serviceName, decimal price, string plan, string persons)
    {
        try
        {
            // Создаем или находим продукт для услуги обслуживания
            var slug = $"server-maintenance-{plan.ToLower()}-{persons.Replace("-", "")}";
            
            // Сначала ищем по slug
            var maintenanceProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Slug == slug);
            
            // Если не нашли по slug, ищем по имени (для совместимости со старыми записями)
            if (maintenanceProduct == null)
            {
                maintenanceProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name == serviceName && p.Type == ProductType.VPN);
            }

            if (maintenanceProduct == null)
            {
                maintenanceProduct = new Product
                {
                    Name = serviceName,
                    Slug = slug,
                    Type = ProductType.VPN, // Используем VPN тип для услуг обслуживания
                    Location = "Global",
                    PricePerMonth = price,
                    Configuration = $"Server Maintenance Service - {plan} plan for {persons} persons",
                    IsAvailable = true,
                    IsPublished = false, // Не показываем в каталоге
                    ImageUrl = "/images/maintenance.png"
                };
                _context.Products.Add(maintenanceProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                bool hasChanges = false;
                
                // Обновляем slug, если он не совпадает (для старых записей)
                if (maintenanceProduct.Slug != slug)
                {
                    maintenanceProduct.Slug = slug;
                    hasChanges = true;
                }
                
                // Обновляем цену, если она изменилась
                if (maintenanceProduct.PricePerMonth != price)
                {
                    maintenanceProduct.PricePerMonth = price;
                    hasChanges = true;
                }
                
                // Всегда обновляем изображение для услуг обслуживания
                if (maintenanceProduct.ImageUrl != "/images/maintenance.png")
                {
                    maintenanceProduct.ImageUrl = "/images/maintenance.png";
                    hasChanges = true;
                }
                
                // Обновляем имя, если оно изменилось
                if (maintenanceProduct.Name != serviceName)
                {
                    maintenanceProduct.Name = serviceName;
                    hasChanges = true;
                }
                
                // Сохраняем изменения, если они есть
                if (hasChanges)
                {
                    await _context.SaveChangesAsync();
                }
            }

            var cart = GetCart();
            cart.Add(new OrderItem 
            { 
                ProductId = maintenanceProduct.Id, 
                ProductVariantId = null, 
                Price = price 
            });
            
            SaveCart(cart);

            return Json(new { success = true, message = GetLocalizer()["ProductAddedToCart"].Value, itemsAdded = 1 });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error adding maintenance service to cart: {ex.Message}");
            return Json(new { success = false, message = GetLocalizer()["ErrorOccurred"].Value });
        }
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
               || (Request.Headers.ContainsKey("Accept") 
                   && Request.Headers["Accept"].ToString().Contains("application/json"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddITRemoteSupport(string serviceName, decimal price, string plan)
    {
        try
        {
            var slug = $"it-remote-support-{plan.ToLower()}";
            var itRemoteProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (itRemoteProduct == null)
            {
                // Fallback: try to find by name if slug doesn't exist (for older entries)
                itRemoteProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name == serviceName && p.Type == ProductType.VPN);
            }

            if (itRemoteProduct == null)
            {
                itRemoteProduct = new Product
                {
                    Name = serviceName,
                    Slug = slug,
                    Type = ProductType.VPN,
                    Location = "Global",
                    PricePerMonth = price,
                    Configuration = $"IT Remote Support Service - {plan} plan",
                    IsAvailable = true,
                    IsPublished = false,
                    ImageUrl = "/images/itremote.png" // Use itremote.png for IT Remote Support
                };
                _context.Products.Add(itRemoteProduct);
            }
            else
            {
                bool hasChanges = false;

                if (itRemoteProduct.Slug != slug)
                {
                    itRemoteProduct.Slug = slug;
                    hasChanges = true;
                }
                if (itRemoteProduct.Name != serviceName)
                {
                    itRemoteProduct.Name = serviceName;
                    hasChanges = true;
                }
                if (itRemoteProduct.PricePerMonth != price)
                {
                    itRemoteProduct.PricePerMonth = price;
                    hasChanges = true;
                }
                // Always ensure the image is set
                if (itRemoteProduct.ImageUrl != "/images/itremote.png")
                {
                    itRemoteProduct.ImageUrl = "/images/itremote.png";
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    _context.Products.Update(itRemoteProduct);
                }
            }
            await _context.SaveChangesAsync();

            var cart = GetCart();
            cart.Add(new OrderItem
            {
                ProductId = itRemoteProduct.Id,
                ProductVariantId = null,
                Price = price
            });

            SaveCart(cart);

            return Json(new { success = true, message = GetLocalizer()["ProductAddedToCart"].Value, itemsAdded = 1 });
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error adding IT Remote Support service to cart: {ex.Message}");
            return Json(new { success = false, message = GetLocalizer()["ErrorOccurred"].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddConfigurationService(string serviceName, decimal price, string category, string? imageUrl = null)
    {
        // Ищем или создаем продукт для услуги настройки
        var slug = $"config-{category.ToLower()}-{serviceName.ToLower().Replace(" ", "-")}";
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Slug == slug);

        // Определяем изображение на основе переданного imageUrl или категории и названия
        string? productImageUrl = imageUrl;
        if (string.IsNullOrEmpty(productImageUrl))
        {
            // Определяем изображение по категории и названию
            if (category.Equals("CHR", StringComparison.OrdinalIgnoreCase))
            {
                if (serviceName.Contains("Basic", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/chr1.png";
                else if (serviceName.Contains("Standard", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/chr2.png";
                else if (serviceName.Contains("Pro", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/chr3.png";
            }
            else if (category.Equals("MikroTik", StringComparison.OrdinalIgnoreCase))
            {
                if (serviceName.Contains("Basic", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/mikrotik1.png";
                else if (serviceName.Contains("Advanced", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/mikrotik2.png";
                else if (serviceName.Contains("Pro", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/mikrotik3.png";
            }
            else if (category.Equals("Fortinet", StringComparison.OrdinalIgnoreCase))
            {
                if (serviceName.Contains("Basic", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/fortinet1.png";
                else if (serviceName.Contains("Security", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/fortinet2.png";
                else if (serviceName.Contains("Enterprise", StringComparison.OrdinalIgnoreCase))
                    productImageUrl = "/images/fortinet3.png";
            }
        }

        if (product == null)
        {
            // Создаем временный продукт для услуги настройки
            product = new Product
            {
                Name = serviceName,
                Slug = slug,
                Type = ProductType.VPN, // Используем VPN тип для услуг настройки
                Location = "Remote",
                PricePerMonth = price,
                Configuration = $"Configuration service: {category}",
                IsAvailable = true,
                IsPublished = false, // Не показываем в каталоге
                ImageUrl = productImageUrl // Используем оригинальное изображение товара
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Обновляем изображение, если его нет или оно неправильное
            if (string.IsNullOrEmpty(product.ImageUrl) || product.ImageUrl == "/images/setupWinserv.png")
            {
                if (!string.IsNullOrEmpty(productImageUrl))
                {
                    product.ImageUrl = productImageUrl;
                    await _context.SaveChangesAsync();
                }
            }
        }

        var cart = GetCart();
        cart.Add(new OrderItem { ProductId = product.Id, ProductVariantId = null, Price = price });
        SaveCart(cart);

        // Проверяем, является ли это AJAX запросом
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
        {
            return Json(new { success = true, message = "Service added to cart" });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAll()
    {
        var products = await _context.Products
            .Include(p => p.Variants)
            .Where(p => p.IsPublished && p.IsAvailable)
            .AsNoTracking()
            .ToListAsync();

        var cart = GetCart();

        foreach (var product in products)
        {
            var defaultVariant = product.Variants.OrderBy(v => v.Id).FirstOrDefault();
            var price = defaultVariant?.Price ?? product.PricePerMonth;
            cart.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductVariantId = defaultVariant?.Id,
                Price = price
            });
        }

        SaveCart(cart);
        TempData["Success"] = "All products added to cart.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int index)
    {
        var cart = GetCart();
        if (index >= 0 && index < cart.Count)
        {
            cart.RemoveAt(index);
            SaveCart(cart);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        var cart = GetCart();
        if (!cart.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        var order = new Order
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
            Items = cart,
            Total = cart.Sum(i => i.Price),
            Currency = "USD",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
}
