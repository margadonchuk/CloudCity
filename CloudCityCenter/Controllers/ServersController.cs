using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

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
    /// Displays static dedicated server plans.
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
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

