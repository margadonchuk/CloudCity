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

        // Если товаров нет, возвращаем пустую модель с инициализированными списками
        if (!products.Any())
        {
            return View(new WindowsServerPageVm
            {
                PlansForFiveToEightPersons = new List<WindowsServerPlanVm>(),
                PlansForFifteenPersons = new List<WindowsServerPlanVm>(),
                PlansForTwentyFivePersons = new List<WindowsServerPlanVm>(),
                PlansForThirtyFivePersons = new List<WindowsServerPlanVm>(),
                PlansForFiftyPersons = new List<WindowsServerPlanVm>(),
                AvailableLocations = new List<string>(),
                AvailablePersons = new List<int>(),
                AvailableCpu = new List<string>(),
                AvailableRam = new List<string>(),
                AvailableSsd = new List<string>()
            });
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

        // Преобразуем товары для 5-8 человек в список планов
        var plansForFiveToEight = productsForFiveToEight.Select(p =>
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
        .ToList();

        // Преобразуем товары для 15 человек в список планов
        var plansForFifteen = productsForFifteen.Select(p =>
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
        .ToList();

        // Преобразуем товары для 25 человек в список планов
        var plansForTwentyFive = productsForTwentyFive.Select(p =>
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
        .ToList();

        // Преобразуем товары для 35 человек в список планов
        var plansForThirtyFive = productsForThirtyFive.Select(p =>
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
        .ToList();

        // Преобразуем товары для 50 человек в список планов
        var plansForFifty = productsForFifty.Select(p =>
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
        .ToList();

            // Собираем все планы для получения уникальных значений фильтров
            var allPlans = new List<WindowsServerPlanVm>();
            if (plansForFiveToEight != null) allPlans.AddRange(plansForFiveToEight);
            if (plansForFifteen != null) allPlans.AddRange(plansForFifteen);
            if (plansForTwentyFive != null) allPlans.AddRange(plansForTwentyFive);
            if (plansForThirtyFive != null) allPlans.AddRange(plansForThirtyFive);
            if (plansForFifty != null) allPlans.AddRange(plansForFifty);

            var vm = new WindowsServerPageVm
            {
                PlansForFiveToEightPersons = plansForFiveToEight ?? new List<WindowsServerPlanVm>(),
                PlansForFifteenPersons = plansForFifteen ?? new List<WindowsServerPlanVm>(),
                PlansForTwentyFivePersons = plansForTwentyFive ?? new List<WindowsServerPlanVm>(),
                PlansForThirtyFivePersons = plansForThirtyFive ?? new List<WindowsServerPlanVm>(),
                PlansForFiftyPersons = plansForFifty ?? new List<WindowsServerPlanVm>(),
                AvailableLocations = allPlans?.Select(p => p?.Country ?? string.Empty).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(l => l).ToList() ?? new List<string>(),
                AvailablePersons = allPlans?.Select(p => p.NumberOfPersons).Distinct().OrderBy(p => p).ToList() ?? new List<int>(),
                AvailableCpu = allPlans?.Select(p => p?.CPU ?? string.Empty).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToList() ?? new List<string>(),
                AvailableRam = allPlans?.Select(p => p?.RAM ?? string.Empty).Where(r => !string.IsNullOrEmpty(r)).Distinct().OrderBy(r => r).ToList() ?? new List<string>(),
                AvailableSsd = allPlans?.Select(p => p?.SSD ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList() ?? new List<string>()
            };

            return View(vm);
        }
        catch (Exception ex)
        {
            // Логируем ошибку
            Console.Error.WriteLine($"Error in ServersController.Index: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.Error.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            
            // Возвращаем пустую модель с инициализированными списками
            return View(new WindowsServerPageVm
            {
                PlansForFiveToEightPersons = new List<WindowsServerPlanVm>(),
                PlansForFifteenPersons = new List<WindowsServerPlanVm>(),
                PlansForTwentyFivePersons = new List<WindowsServerPlanVm>(),
                PlansForThirtyFivePersons = new List<WindowsServerPlanVm>(),
                PlansForFiftyPersons = new List<WindowsServerPlanVm>(),
                AvailableLocations = new List<string>(),
                AvailablePersons = new List<int>(),
                AvailableCpu = new List<string>(),
                AvailableRam = new List<string>(),
                AvailableSsd = new List<string>()
            });
        }
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

