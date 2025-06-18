using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

namespace CloudCityCenter.Controllers;

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
            .Include(o => o.Server)
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
            .Include(o => o.Server)
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
        ViewData["ServerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Servers, "Id", "Name");
        return View();
    }

    // POST: Orders/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ServerId,TotalPrice")] Order order)
    {
        order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        order.OrderDate = DateTime.UtcNow;
        if (ModelState.IsValid)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ServerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Servers, "Id", "Name", order.ServerId);
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
        ViewData["ServerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Servers, "Id", "Name", order.ServerId);
        return View(order);
    }

    // POST: Orders/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,ServerId,TotalPrice")] Order order)
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
        ViewData["ServerId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Servers, "Id", "Name", order.ServerId);
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
            .Include(o => o.Server)
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

