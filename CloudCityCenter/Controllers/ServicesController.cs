using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

[AllowAnonymous]
public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.Type == ProductType.DedicatedServer
                        || p.Type == ProductType.VPS
                        || p.Type == ProductType.VPN)
            .ToListAsync();
        var viewModel = new ServiceIndexViewModel { Products = products };
        return View(viewModel);
    }
}
