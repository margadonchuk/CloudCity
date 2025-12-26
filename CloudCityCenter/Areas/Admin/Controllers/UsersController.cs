using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudCityCenter.Data;
using Microsoft.AspNetCore.Identity;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<UsersController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Admin/Users/NewUsers
    public async Task<IActionResult> NewUsers()
    {
        // Получаем пользователей, отсортированных по дате регистрации (новые первыми)
        // IdentityUser имеет свойство, но нам нужно получить их из базы
        var users = await _userManager.Users
            .OrderByDescending(u => u.Id) // Сортируем по ID (обычно новые имеют больший ID)
            .Take(100) // Ограничиваем до 100 последних
            .ToListAsync();

        ViewData["Title"] = "New Users";
        return View(users);
    }

    // GET: Admin/Users/Customers
    public async Task<IActionResult> Customers()
    {
        // Получаем пользователей, которые сделали хотя бы один заказ
        var userIdsWithOrders = await _context.Orders
            .Select(o => o.UserId)
            .Distinct()
            .ToListAsync();

        var customers = await _userManager.Users
            .Where(u => userIdsWithOrders.Contains(u.Id))
            .ToListAsync();

        // Получаем дополнительную информацию о заказах для каждого пользователя
        var customersWithInfo = new List<CustomerViewModel>();
        
        foreach (var user in customers)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .ToListAsync();
            
            var lastOrder = orders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
            
            customersWithInfo.Add(new CustomerViewModel
            {
                User = user,
                OrdersCount = orders.Count,
                TotalSpent = orders
                    .Where(o => o.Status == Models.OrderStatus.Completed)
                    .Sum(o => o.Total),
                LastOrderDate = lastOrder?.CreatedAt
            });
        }

        ViewData["Title"] = "Customers";
        
        // Сортируем по дате последнего заказа
        var sortedCustomers = customersWithInfo
            .OrderByDescending(c => c.LastOrderDate)
            .Select(c => c.User)
            .ToList();
        
        ViewData["CustomersInfo"] = customersWithInfo.ToDictionary(
            c => c.User.Id,
            c => c
        );
        
        return View(sortedCustomers);
    }

    private class CustomerViewModel
    {
        public IdentityUser User { get; set; } = null!;
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
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

        // Получаем заказы пользователя
        var orders = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == id)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        ViewData["Orders"] = orders;
        ViewData["OrdersCount"] = orders.Count;
        ViewData["TotalSpent"] = orders
            .Where(o => o.Status == Models.OrderStatus.Completed)
            .Sum(o => o.Total);

        return View(user);
    }
}

