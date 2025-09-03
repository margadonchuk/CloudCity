using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

/// <summary>
/// Public controller for browsing available servers.
/// </summary>
[Route("[controller]")]
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays dedicated server products.
    /// </summary>
    [HttpGet]
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
                    .ToList(),
            })
            .ToListAsync();

        return View(servers);
    }

    /// <summary>
    /// Shows details for a server identified by slug.
    /// </summary>
    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var server = await _context.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive);

        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }
}

