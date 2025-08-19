using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

[Authorize(Roles = "Admin")]
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Servers
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var servers = await _context.Products
            .Where(p => p.Type == ProductType.DedicatedServer && p.IsPublished)
            .Select(p => new ProductCardVm
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                PricePerMonth = p.PricePerMonth,
                ImageUrl = p.ImageUrl,
                TopFeatures = p.Features
                    .OrderBy(f => f.Id)
                    .Select(f => string.IsNullOrWhiteSpace(f.Value) ? f.Name : $"{f.Name}: {f.Value}")
                    .Take(3)
                    .ToList()
            })
            .ToListAsync();

        return View(servers);
    }

    // GET: Servers/Details/slug
    [AllowAnonymous]
    public async Task<IActionResult> Details(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var server = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Features)
            .Where(p => p.Type == ProductType.DedicatedServer && p.IsPublished)
            .FirstOrDefaultAsync(m => m.Slug == slug);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // GET: Servers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Servers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Location,PricePerMonth,Configuration,IsAvailable,ImageUrl,Type")] Product server)
    {
        if (ModelState.IsValid)
        {
            server.Type = ProductType.DedicatedServer;
            _context.Add(server);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(server);
    }

    // GET: Servers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Products
            .Where(p => p.Type == ProductType.DedicatedServer)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (server == null)
        {
            return NotFound();
        }
        return View(server);
    }

    // POST: Servers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,PricePerMonth,Configuration,IsAvailable,ImageUrl,Type")] Product server)
    {
        if (id != server.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                server.Type = ProductType.DedicatedServer;
                _context.Update(server);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(server.Id))
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
        return View(server);
    }

    // GET: Servers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Products
            .Where(p => p.Type == ProductType.DedicatedServer)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // POST: Servers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var server = await _context.Products.FindAsync(id);
        if (server != null)
        {
            _context.Products.Remove(server);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ServerExists(int id)
    {
        return _context.Products.Any(e => e.Id == id && e.Type == ProductType.DedicatedServer);
    }
}
