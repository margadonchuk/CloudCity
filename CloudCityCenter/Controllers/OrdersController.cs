using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace CloudCityCenter.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Orders
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var orders = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Items).ThenInclude(i => i.ProductVariant)
            .Where(o => o.UserId == userId)
            .ToListAsync();
        
        // SEO оптимизация (для заказов менее критично, так как требуется авторизация)
        ViewData["Title"] = "Мои заказы";
        ViewData["Description"] = "Ваши заказы на аренду серверов.";
        
        return View(orders);
    }

    // GET: Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Items).ThenInclude(i => i.ProductVariant)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (order == null)
        {
            return NotFound();
        }

        // SEO оптимизация
        ViewData["Title"] = $"Детали заказа #{order.Id}";
        ViewData["Description"] = "Детали вашего заказа на аренду серверов.";
        
        return View(order);
    }

    // GET: Orders/Create
    public IActionResult Create()
    {
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
        ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Name");
        return View(new Order { Items = { new OrderItem() } });
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Items,Total,Currency,Status")] Order order)
    {
        order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        order.Currency = "USD"; // Устанавливаем доллары по умолчанию
        if (order.Items != null && order.Items.Count > 0 && order.Total == 0)
        {
            order.Total = order.Items.Sum(i => i.Price);
        }
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", order.Items?.FirstOrDefault()?.ProductId);
        ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Name", order.Items?.FirstOrDefault()?.ProductVariantId);
        return View(order);
    }

    // GET: Orders/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }

    // POST: Orders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Total,Currency,Status")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (existing == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existing.Total = order.Total;
                existing.Currency = order.Currency;
                existing.Status = order.Status;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(order);
    }

    // GET: Orders/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: Orders/Pay/5
    public async Task<IActionResult> Pay(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var order = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Items).ThenInclude(i => i.ProductVariant)
            .Include(o => o.User)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        
        if (order == null)
        {
            return NotFound();
        }

        // Формируем сообщение с данными заказа
        var message = $"💳 *Заявка на оплату заказа*\n\n";
        message += $"📋 *Номер заказа:* #{order.Id}\n";
        message += $"👤 *Пользователь:* {order.User?.Email ?? "Не указан"}\n";
        message += $"📅 *Дата создания:* {order.CreatedAt:dd.MM.yyyy HH:mm}\n";
        message += $"💰 *Сумма:* ${order.Total:F2} {order.Currency}\n";
        message += $"📊 *Статус:* {order.Status}\n\n";
        message += $"*Товары:*\n";
        
        foreach (var item in order.Items)
        {
            message += $"• {item.Product?.Name ?? "Не указано"}";
            if (item.ProductVariant != null)
            {
                message += $" - {item.ProductVariant.Name}";
            }
            message += $" - ${item.Price:F2}\n";
        }
        
        message += $"\n🔗 *Ссылка на заказ:* https://cloudcitylife.com/Orders/Details/{order.Id}";

        // URL-кодируем сообщение для Telegram
        var encodedMessage = Uri.EscapeDataString(message);
        var telegramUrl = $"https://t.me/finance_cloudcity?text={encodedMessage}";

        // Перенаправляем на Telegram
        return Redirect(telegramUrl);
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
