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
                        ImageUrl = null
                    };
                    _context.Products.Add(setupProduct);
                    await _context.SaveChangesAsync();
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

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest" 
               || (Request.Headers.ContainsKey("Accept") 
                   && Request.Headers["Accept"].ToString().Contains("application/json"));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddConfigurationService(string serviceName, decimal price, string category)
    {
        // Ищем или создаем продукт для услуги настройки
        var slug = $"config-{category.ToLower()}-{serviceName.ToLower().Replace(" ", "-")}";
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Slug == slug);

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
                ImageUrl = null
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        HttpContext.Session.Remove(CartSessionKey);
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
}
