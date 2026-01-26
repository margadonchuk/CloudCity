using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CloudCityCenter.Models;
using System.Threading.Tasks;
using CloudCityCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizerFactory _localizerFactory;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IStringLocalizerFactory localizerFactory)
    {
        _logger = logger;
        _context = context;
        _localizerFactory = localizerFactory;
    }
    
    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.Home.Index", "CloudCityCenter");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => p.IsAvailable && p.Type == ProductType.DedicatedServer)
            .ToListAsync();
        
        // SEO оптимизация для главной страницы с локализацией
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;
        
        return View(products);
    }

    public IActionResult Privacy()
    {
        // SEO оптимизация
        ViewData["Title"] = "Политика конфиденциальности";
        ViewData["Description"] = "Политика конфиденциальности CloudCityCenter - аренда серверов по всему миру.";
        ViewData["Keywords"] = "политика конфиденциальности, конфиденциальность";
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCode(int code)
    {
        ViewData["StatusCode"] = code;
        ViewData["Title"] = code == StatusCodes.Status403Forbidden
            ? "Form submission limit reached"
            : "Something went wrong";

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
