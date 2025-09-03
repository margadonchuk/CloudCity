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

        return View(vpnProducts);
    }
}
