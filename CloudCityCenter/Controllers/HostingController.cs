using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

public class HostingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizerFactory _localizerFactory;

    public HostingController(ApplicationDbContext context, IStringLocalizerFactory localizerFactory)
    {
        _context = context;
        _localizerFactory = localizerFactory;
    }
    
    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.Hosting.Index", "CloudCityCenter");
    }

    public async Task<IActionResult> Index()
    {
        var hostingPlans = await LoadProductCards(ProductType.Hosting);
        var websiteProducts = await LoadProductCards(ProductType.Website);
        var vpsProducts = await LoadProductCards(ProductType.VPS);
        var vpnProducts = await LoadProductCards(ProductType.VPN);
        var storageProducts = await LoadProductCards(ProductType.Storage);
        var websiteBuilderProducts = await LoadProductCards(ProductType.WebsiteBuilder);
        var websiteCodeProducts = await LoadProductCards(ProductType.WebsiteCode);

        var vm = new HostingPageVm
        {
            HostingPlans = hostingPlans,
            WebsiteProducts = websiteProducts,
            VpsProducts = vpsProducts,
            VpnProducts = vpnProducts,
            StorageProducts = storageProducts,
            WebsiteBuilderProducts = websiteBuilderProducts,
            WebsiteCodeProducts = websiteCodeProducts
        };

        // SEO оптимизация с локализацией
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;
        
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

            // Порядок отображения фичей в зависимости от типа товара
            string[] featureOrder;
            int maxFeatures;
            if (type == ProductType.Storage)
            {
                featureOrder = new[] { "Storage", "CPU", "RAM", "Access", "Security", "Backup", "Support" };
                maxFeatures = 7;
            }
            else if (type == ProductType.Hosting)
            {
                featureOrder = new[] { "CPU", "RAM", "SSD (NVMe)", "OS", "Bandwidth", "Country" };
                maxFeatures = 6;
            }
            else if (type == ProductType.WebsiteBuilder || type == ProductType.WebsiteCode)
            {
                featureOrder = new[] { "Description", "Included", "Technologies", "Price" };
                maxFeatures = 10;
            }
            else
            {
                featureOrder = Array.Empty<string>();
                maxFeatures = 6;
            }

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
    }
}
