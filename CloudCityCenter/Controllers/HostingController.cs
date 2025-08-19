using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class HostingController : Controller
{
    private readonly ApplicationDbContext _context;

    public HostingController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hostingPlans = await _context.Products
            .Where(p => p.Type == ProductType.Hosting && p.IsPublished)
            .Select(p => new ProductCardVm
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Location = p.Location,
                PricePerMonth = p.PricePerMonth,
                Configuration = p.Configuration,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        var websiteProducts = await _context.Products
            .Where(p => p.Type == ProductType.Website && p.IsPublished)
            .Select(p => new ProductCardVm
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Location = p.Location,
                PricePerMonth = p.PricePerMonth,
                Configuration = p.Configuration,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();

        var vm = new HostingPageVm
        {
            HostingPlans = hostingPlans,
            WebsiteProducts = websiteProducts
        };

        return View(vm);
    }
}
