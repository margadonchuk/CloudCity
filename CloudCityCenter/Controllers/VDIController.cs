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
            .ToListAsync();

        // Группируем товары по регионам
        var regions = products
            .GroupBy(p => p.Location)
            .Select(g => new VDIPlansByRegionVm
            {
                RegionName = g.Key,
                Plans = g.Select(p =>
                {
                    var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value ?? "");
                    return new VDIPlanVm
                    {
                        Name = ExtractPlanName(p.Name), // Извлекаем "Start", "Standard" или "Pro"
                        CpuCores = ParseFeature(featuresDict, "CPU", "core"),
                        RamGb = ParseFeature(featuresDict, "RAM", "GB"),
                        SsdGb = ParseFeature(featuresDict, "SSD", "GB"),
                        Traffic = featuresDict.GetValueOrDefault("Traffic", "1 Gb/s"),
                        Country = featuresDict.GetValueOrDefault("Country", p.Location),
                        Price = p.PricePerMonth,
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
            Regions = regions
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

