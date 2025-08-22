using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServersController> _logger;

    public ServersController(ApplicationDbContext context, ILogger<ServersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Servers
    public async Task<IActionResult> Index()
    {
        var servers = await _context.Servers.AsNoTracking().ToListAsync();
        return View(servers);
    }

    // GET: Admin/Servers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // GET: Admin/Servers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Servers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Slug,Description,Location,PricePerMonth,CPU,RamGb,StorageGb,ImageUrl,IsActive,DDoSTier,Stock")] Server server)
    {
        if (!ModelState.IsValid)
        {
            return View(server);
        }

        try
        {
            _context.Add(server);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Server {ServerSlug} created", server.Slug);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                ex.InnerException?.Message.Contains("IX_Servers_Slug") == true)
            {
                ModelState.AddModelError("Slug", "Slug already exists.");
                return View(server);
            }

            throw;
        }
    }

    // GET: Admin/Servers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.FindAsync(id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // POST: Admin/Servers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Description,Location,PricePerMonth,CPU,RamGb,StorageGb,ImageUrl,IsActive,DDoSTier,Stock")] Server server)
    {
        if (id != server.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(server);
        }

        try
        {
            _context.Update(server);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Server {ServerSlug} edited", server.Slug);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                ex.InnerException?.Message.Contains("IX_Servers_Slug") == true)
            {
                ModelState.AddModelError("Slug", "Slug already exists.");
                return View(server);
            }

            throw;
        }
    }

    // GET: Admin/Servers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // POST: Admin/Servers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var server = await _context.Servers.FindAsync(id);
        if (server != null)
        {
            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Server {ServerSlug} deleted", server.Slug);
        }
        return RedirectToAction(nameof(Index));
    }
}

