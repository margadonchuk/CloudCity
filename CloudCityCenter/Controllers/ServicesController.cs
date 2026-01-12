using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

[AllowAnonymous]
public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizerFactory _localizerFactory;

    public ServicesController(ApplicationDbContext context, IStringLocalizerFactory localizerFactory)
    {
        _context = context;
        _localizerFactory = localizerFactory;
    }
    
    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.Services.Index", "CloudCityCenter");
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
        
        // SEO оптимизация с локализацией
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;
        
        return View(viewModel);
    }
}
