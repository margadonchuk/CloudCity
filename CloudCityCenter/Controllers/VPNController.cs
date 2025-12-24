using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class VPNController : Controller
{
    private readonly ApplicationDbContext _context;

    public VPNController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vpnProducts = await _context.Products
            .Where(p => p.Type == ProductType.VPN && p.IsPublished)
            .Include(p => p.Features)
            .ToListAsync();

        var productCards = vpnProducts.Select(p =>
        {
            var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value);

            // Порядок отображения фичей для VPN продуктов
            string[] featureOrder = new[] { "Technology", "Devices", "Traffic", "Encryption", "Country" };
            int maxFeatures = 10;

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
                TopFeatures = orderedFeatures.Take(maxFeatures).ToList()
            };
        }).ToList();

        return View(productCards);
    }
}
