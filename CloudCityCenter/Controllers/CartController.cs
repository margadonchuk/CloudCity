using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private const string CartSessionKey = "Cart";

    public CartController(ApplicationDbContext context)
    {
        _context = context;
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

        var products = await _context.Products.Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);
        var variants = await _context.ProductVariants.Where(v => variantIds.Contains(v.Id))
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
    public IActionResult Add(int productId, int? productVariantId, decimal price)
    {
        var cart = GetCart();
        cart.Add(new OrderItem { ProductId = productId, ProductVariantId = productVariantId, Price = price });
        SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
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
