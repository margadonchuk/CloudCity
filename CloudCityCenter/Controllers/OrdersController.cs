using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

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
            .Include(o => o.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
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
            .Include(o => o.Product)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: Orders/Create
    public IActionResult Create()
    {
        ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products.Where(p => p.Type == ProductType.DedicatedServer), "Id", "Name");
        return View();
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductId,TotalPrice,Status")] Order order)
    {
        order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        order.OrderDate = DateTime.UtcNow;
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products.Where(p => p.Type == ProductType.DedicatedServer), "Id", "Name", order.ProductId);
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
        var order = await _context.Orders.FindAsync(id);
        if (order == null || order.UserId != userId)
        {
            return NotFound();
        }
        ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products.Where(p => p.Type == ProductType.DedicatedServer), "Id", "Name", order.ProductId);
        return View(order);
    }

    // POST: Orders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,TotalPrice,Status")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (existing == null)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                order.UserId = existing.UserId;
                order.OrderDate = existing.OrderDate;
                _context.Update(order);
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
        ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products.Where(p => p.Type == ProductType.DedicatedServer), "Id", "Name", order.ProductId);
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
            .Include(o => o.Product)
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

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}

