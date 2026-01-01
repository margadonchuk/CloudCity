using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Admin/Users/NewUsers
    public async Task<IActionResult> NewUsers()
    {
        // Пользователи, зарегистрированные за последние 30 дней
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        
        var newUsers = await _userManager.Users
            .Where(u => u.LockoutEnd == null || u.LockoutEnd > DateTimeOffset.UtcNow)
            .OrderByDescending(u => u.Id) // Примерная сортировка по дате регистрации
            .Take(100) // Ограничиваем для производительности
            .ToListAsync();

        return View(newUsers);
    }

    // GET: Admin/Users/Customers
    public async Task<IActionResult> Customers()
    {
        // Пользователи, которые сделали хотя бы один заказ
        var customerIds = await _context.Orders
            .Select(o => o.UserId)
            .Distinct()
            .ToListAsync();

        var customers = await _userManager.Users
            .Where(u => customerIds.Contains(u.Id))
            .Select(u => new CustomerViewModel
            {
                User = u,
                OrderCount = _context.Orders.Count(o => o.UserId == u.Id),
                TotalSpent = _context.Orders
                    .Where(o => o.UserId == u.Id)
                    .Sum(o => o.Total)
            })
            .OrderByDescending(c => c.TotalSpent)
            .ToListAsync();

        return View(customers);
    }

    // GET: Admin/Users/Details/5
    public async Task<IActionResult> Details(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var orders = await _context.Orders
            .Where(o => o.UserId == id)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        ViewData["Orders"] = orders;
        ViewData["OrderCount"] = orders.Count;
        ViewData["TotalSpent"] = orders.Sum(o => o.Total);

        return View(user);
    }
}

public class CustomerViewModel
{
    public IdentityUser User { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}

