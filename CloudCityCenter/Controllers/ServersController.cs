using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

/// <summary>
/// Public controller for browsing available servers.
/// </summary>
[Route("[controller]")]
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays dedicated server products grouped by number of persons.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.Type == ProductType.DedicatedServer && p.IsPublished)
            .Include(p => p.Features)
            .Include(p => p.Variants)
            .ToListAsync();

        // Если товаров нет, возвращаем пустую модель
        if (!products.Any())
        {
            return View(new WindowsServerPageVm());
        }

        // Разделяем товары на планы для 5-8, 15, 25, 35 и 50 человек
        // Определяем по Configuration или Slug
        
        // Товары для 50 человек определяются по наличию "50" в Configuration или "-50" в Slug
        var productsForFifty = products.Where(p => 
            (p.Configuration != null && p.Configuration.Contains("50", StringComparison.OrdinalIgnoreCase)) ||
            (p.Slug != null && p.Slug.Contains("-50"))
        ).ToList();
        
        // Товары для 35 человек определяются по наличию "35" в Configuration или "-35" в Slug
        var productsForThirtyFive = products.Where(p => 
            !productsForFifty.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("35", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && p.Slug.Contains("-35")))
        ).ToList();
        
        // Товары для 25 человек определяются по наличию "25" в Configuration или "-25" в Slug
        var productsForTwentyFive = products.Where(p => 
            !productsForFifty.Contains(p) && !productsForThirtyFive.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("25", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && p.Slug.Contains("-25")))
        ).ToList();
        
        // Товары для 15 человек определяются по наличию "15" в Configuration или "-15" в Slug
        var productsForFifteen = products.Where(p => 
            !productsForFifty.Contains(p) && !productsForThirtyFive.Contains(p) && !productsForTwentyFive.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("15", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && p.Slug.Contains("-15")))
        ).ToList();
        
        // Товары для 5-8 человек - остальные (содержат "5-8" в Configuration или "5-8" в Slug)
        var productsForFiveToEight = products.Where(p => 
            !productsForFifteen.Contains(p) && !productsForTwentyFive.Contains(p) && 
            !productsForThirtyFive.Contains(p) && !productsForFifty.Contains(p) &&
            ((p.Configuration != null && p.Configuration.Contains("5-8", StringComparison.OrdinalIgnoreCase)) ||
             (p.Slug != null && p.Slug.Contains("5-8")))
        ).ToList();

        // Группируем товары для 5-8 человек по регионам
        var regionsForFiveToEight = productsForFiveToEight
            .GroupBy(p => p.Location)
            .Select(g => new WindowsServerPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new WindowsServerPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = p.Name,
                        CPU = featuresDict.GetValueOrDefault("CPU", ""),
                        RAM = featuresDict.GetValueOrDefault("RAM", ""),
                        SSD = featuresDict.GetValueOrDefault("SSD", ""),
                        Network = featuresDict.GetValueOrDefault("Network", ""),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl,
                        NumberOfPersons = 5
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 15 человек по регионам
        var regionsForFifteen = productsForFifteen
            .GroupBy(p => p.Location)
            .Select(g => new WindowsServerPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new WindowsServerPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = p.Name,
                        CPU = featuresDict.GetValueOrDefault("CPU", ""),
                        RAM = featuresDict.GetValueOrDefault("RAM", ""),
                        SSD = featuresDict.GetValueOrDefault("SSD", ""),
                        Network = featuresDict.GetValueOrDefault("Network", ""),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl,
                        NumberOfPersons = 15
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 25 человек по регионам
        var regionsForTwentyFive = productsForTwentyFive
            .GroupBy(p => p.Location)
            .Select(g => new WindowsServerPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new WindowsServerPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = p.Name,
                        CPU = featuresDict.GetValueOrDefault("CPU", ""),
                        RAM = featuresDict.GetValueOrDefault("RAM", ""),
                        SSD = featuresDict.GetValueOrDefault("SSD", ""),
                        Network = featuresDict.GetValueOrDefault("Network", ""),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl,
                        NumberOfPersons = 25
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 35 человек по регионам
        var regionsForThirtyFive = productsForThirtyFive
            .GroupBy(p => p.Location)
            .Select(g => new WindowsServerPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new WindowsServerPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = p.Name,
                        CPU = featuresDict.GetValueOrDefault("CPU", ""),
                        RAM = featuresDict.GetValueOrDefault("RAM", ""),
                        SSD = featuresDict.GetValueOrDefault("SSD", ""),
                        Network = featuresDict.GetValueOrDefault("Network", ""),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl,
                        NumberOfPersons = 35
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        // Группируем товары для 50 человек по регионам
        var regionsForFifty = productsForFifty
            .GroupBy(p => p.Location)
            .Select(g => new WindowsServerPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    var defaultVariant = p.Variants.OrderBy(v => v.Id).FirstOrDefault();
                    return new WindowsServerPlanVm
                    {
                        ProductId = p.Id,
                        ProductVariantId = defaultVariant?.Id,
                        Name = p.Name,
                        CPU = featuresDict.GetValueOrDefault("CPU", ""),
                        RAM = featuresDict.GetValueOrDefault("RAM", ""),
                        SSD = featuresDict.GetValueOrDefault("SSD", ""),
                        Network = featuresDict.GetValueOrDefault("Network", ""),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = defaultVariant?.Price ?? p.PricePerMonth,
                        ImageUrl = p.ImageUrl,
                        NumberOfPersons = 50
                    };
                })
                .OrderBy(p => p.Price)
                .ToList()
            })
            .OrderBy(r => r.RegionName)
            .ToList();

        var vm = new WindowsServerPageVm
        {
            RegionsForFiveToEightPersons = regionsForFiveToEight,
            RegionsForFifteenPersons = regionsForFifteen,
            RegionsForTwentyFivePersons = regionsForTwentyFive,
            RegionsForThirtyFivePersons = regionsForThirtyFive,
            RegionsForFiftyPersons = regionsForFifty
        };

        return View(vm);
    }

    /// <summary>
    /// Shows details for a server identified by slug.
    /// </summary>
    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var server = await _context.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive);

        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }
}

