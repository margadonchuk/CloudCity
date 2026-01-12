using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

public class VPSController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizerFactory _localizerFactory;

    public VPSController(ApplicationDbContext context, IStringLocalizerFactory localizerFactory)
    {
        _context = context;
        _localizerFactory = localizerFactory;
    }
    
    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.VPS.Index", "CloudCityCenter");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.Type == ProductType.VPS && p.IsPublished)
            .Include(p => p.Features)
            .Include(p => p.Variants)
            .ToListAsync();

        var plans = products.Select(p =>
        {
            var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
            var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
            return new VPSPlanVm
            {
                ProductId = p.Id,
                ProductVariantId = defaultVariant?.Id,
                Name = p.Name,
                CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                Traffic = featuresDict.GetValueOrDefault("Traffic", ""),
                Country = featuresDict.GetValueOrDefault("Country", p.Location),
                OS = featuresDict.GetValueOrDefault("OS", "Linux"),
                Price = defaultVariant?.Price ?? p.PricePerMonth,
                ImageUrl = p.ImageUrl
            };
        })
        .OrderBy(p => p.Price)
        .ToList();

        var vm = new VPSPageVm
        {
            Plans = plans,
            AvailableCpuCores = plans.Select(p => p.CpuCores).Distinct().OrderBy(c => c).ToList(),
            AvailableRamGb = plans.Select(p => p.RamGb).Distinct().OrderBy(r => r).ToList(),
            AvailableSsdGb = plans.Select(p => p.SsdGb).Distinct().OrderBy(s => s).ToList(),
            AvailableTraffic = plans.Select(p => p.Traffic).Distinct().OrderBy(t => t).ToList()
        };

        // SEO оптимизация с локализацией
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;
        
        return View(vm);
    }

    private int ParseFeature(Dictionary<string, string> features, string key, string unit)
    {
        if (!features.TryGetValue(key, out var value))
            return 0;

        // Извлекаем число из строки типа "1 core" или "1 GB"
        var match = Regex.Match(value, @"(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}

