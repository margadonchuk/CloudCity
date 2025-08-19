using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CloudCityCenter.Models;
using System.Threading.Tasks;
using CloudCityCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsAvailable && p.Type == ProductType.DedicatedServer)
            .ToListAsync();
        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
