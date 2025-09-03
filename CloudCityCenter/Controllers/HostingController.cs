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
        var hostingPlans = await LoadProductCards(ProductType.Hosting);
        var websiteProducts = await LoadProductCards(ProductType.Website);
        var vpsProducts = await LoadProductCards(ProductType.VPS);
        var vpnProducts = await LoadProductCards(ProductType.VPN);

        var vm = new HostingPageVm
        {
            HostingPlans = hostingPlans,
            WebsiteProducts = websiteProducts,
            VpsProducts = vpsProducts,
            VpnProducts = vpnProducts
        };

        return View(vm);
    }

    private async Task<List<ProductCardVm>> LoadProductCards(ProductType type)
    {
        return await _context.Products
            .Where(p => p.Type == type && p.IsPublished)
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
                    .ToList(),
            })
            .ToListAsync();
    }
}
