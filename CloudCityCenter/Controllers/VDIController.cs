using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class VDIController : Controller
{
    private readonly ApplicationDbContext _context;

    public VDIController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.Type == ProductType.VDI && p.IsPublished)
            .Include(p => p.Features)
            .Include(p => p.Variants)
            .ToListAsync();

        // Разделяем товары на планы для 1, 3, 5, 10 и 20 человек
        // Товары для 20 человек определяются по наличию "20" в Configuration или Slug
        var productsForTwenty = products.Where(p => 
            (p.Configuration != null && p.Configuration.Contains("20", StringComparison.OrdinalIgnoreCase)) ||
            (p.Slug != null && (p.Slug.Contains("-20") || p.Slug.EndsWith("-20")))
        ).ToList();
        
        // Товары для 10 человек определяются по наличию "10" в Configuration или Slug
        var productsForTen = products.Where(p => 
            !productsForTwenty.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("10", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && (p.Slug.Contains("-10") || p.Slug.EndsWith("-10"))))
        ).ToList();
        
        // Товары для 5 человек определяются по наличию "5" в Configuration или Slug
        var productsForFive = products.Where(p => 
            !productsForTwenty.Contains(p) && !productsForTen.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("5", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && (p.Slug.Contains("-5") || p.Slug.EndsWith("-5"))))
        ).ToList();
        
        // Товары для 3 человек определяются по наличию "3" в Configuration или Slug
        var productsForThree = products.Where(p => 
            !productsForTwenty.Contains(p) && !productsForTen.Contains(p) && !productsForFive.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("3", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && (p.Slug.Contains("-3") || p.Slug.EndsWith("-3"))))
        ).ToList();
        
        // Все остальные товары - для 1 человека
        var productsForOne = products.Where(p => !productsForThree.Contains(p) && !productsForFive.Contains(p) && !productsForTen.Contains(p) && !productsForTwenty.Contains(p)).ToList();
        
        // Если товары для 1 человека не найдены, но есть VDI товары,
        // показываем все товары как для 1 человека (обратная совместимость)
        if (productsForOne.Count == 0 && products.Count > 0 && productsForThree.Count == 0 && productsForFive.Count == 0 && productsForTen.Count == 0 && productsForTwenty.Count == 0)
        {
            productsForOne = products;
        }

        // Группируем товары для 1 человека по регионам
        var regionsForOne = productsForOne
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new VDIPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = ExtractPlanName(p.Name),
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 3 человек по регионам
        var regionsForThree = productsForThree
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new VDIPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = ExtractPlanName(p.Name),
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 5 человек по регионам
        var regionsForFive = productsForFive
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new VDIPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = ExtractPlanName(p.Name),
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 10 человек по регионам
        var regionsForTen = productsForTen
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new VDIPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = ExtractPlanName(p.Name),
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 20 человек по регионам
        var regionsForTwenty = productsForTwenty
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new VDIPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = ExtractPlanName(p.Name),
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        var vm = new VDIPageVm
        {
            RegionsForOnePerson = regionsForOne,
            RegionsForThreePersons = regionsForThree,
            RegionsForFivePersons = regionsForFive,
            RegionsForTenPersons = regionsForTen,
            RegionsForTwentyPersons = regionsForTwenty
        };

        return View(vm);
    }

    private string ExtractPlanName(string fullName)
    {
        if (fullName.Contains("Start", StringComparison.OrdinalIgnoreCase))
            return "Start";
        if (fullName.Contains("Standard", StringComparison.OrdinalIgnoreCase))
            return "Standard";
        if (fullName.Contains("Pro", StringComparison.OrdinalIgnoreCase))
            return "Pro";
        return fullName;
    }

    private int ParseFeature(Dictionary<string, string> features, string key, string unit)
    {
        if (!features.TryGetValue(key, out var value))
            return 0;

        // Извлекаем число из строки типа "4 core" или "8 GB"
        var match = Regex.Match(value, @"(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }
}

