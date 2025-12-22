using System.Linq;
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
        var productCards = await _context.Products
            .Where(p => p.Type == type && p.IsPublished)
            .Include(p => p.Features)
            .ToListAsync();

        return productCards.Select(p =>
        {
            var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value);

            // Порядок отображения фичей для хостинга
            var featureOrder = new[] { "CPU", "RAM", "SSD (NVMe)", "OS", "Bandwidth", "Гео" };
            var orderedFeatures = new List<string>();

            foreach (var featureName in featureOrder)
            {
                if (featuresDict.TryGetValue(featureName, out var value))
                {
                    orderedFeatures.Add(string.IsNullOrWhiteSpace(value)
                        ? featureName
                        : $"{featureName}: {value}");
                }
            }

            // Если есть другие фичи, которые не в списке, добавляем их в конце
            foreach (var feature in p.Features.OrderBy(f => f.Id))
            {
                if (!featureOrder.Contains(feature.Name) && !orderedFeatures.Any(f => f.StartsWith(feature.Name)))
                {
                    orderedFeatures.Add(string.IsNullOrWhiteSpace(feature.Value)
                        ? feature.Name
                        : $"{feature.Name}: {feature.Value}");
                }
            }

            return new ProductCardVm
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                PricePerMonth = p.PricePerMonth,
                ImageUrl = p.ImageUrl,
                TopFeatures = orderedFeatures.Take(6).ToList()
            };
        }).ToList();
    }
}
