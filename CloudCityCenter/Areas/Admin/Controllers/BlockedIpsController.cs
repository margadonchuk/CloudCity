using System.Net;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlockedIpsController : Controller
{
    private readonly ApplicationDbContext _context;

    public BlockedIpsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var blockedIps = await _context.BlockedIpAddresses
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(blockedIps);
    }

    public IActionResult Create()
    {
        return View(new BlockedIpCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlockedIpCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!TryNormalizeIp(model.IpAddress, out var normalizedIp))
        {
            ModelState.AddModelError(nameof(model.IpAddress), "Please enter a valid IPv4 or IPv6 address.");
            return View(model);
        }

        var alreadyBlocked = await _context.BlockedIpAddresses
            .AnyAsync(x => x.IsActive && x.NormalizedIpAddress == normalizedIp);

        if (alreadyBlocked)
        {
            TempData["ErrorMessage"] = "IP address already exists";
            ModelState.AddModelError(nameof(model.IpAddress), "This IP address is already blocked.");
            return View(model);
        }

        var entity = new BlockedIpAddress
        {
            IpAddress = model.IpAddress.Trim(),
            NormalizedIpAddress = normalizedIp,
            Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim(),
            CreatedBy = User?.Identity?.Name,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.BlockedIpAddresses.Add(entity);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "IP address blocked successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock(int id)
    {
        var entry = await _context.BlockedIpAddresses.FindAsync(id);
        if (entry == null)
        {
            TempData["ErrorMessage"] = "Blocked IP was not found";
            return RedirectToAction(nameof(Index));
        }

        entry.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "IP address removed from block list";
        return RedirectToAction(nameof(Index));
    }

    private static bool TryNormalizeIp(string? rawIp, out string normalizedIp)
    {
        normalizedIp = string.Empty;
        if (string.IsNullOrWhiteSpace(rawIp))
        {
            return false;
        }

        return IPAddress.TryParse(rawIp.Trim(), out var parsedIp) &&
               TryNormalizeIp(parsedIp, out normalizedIp);
    }

    private static bool TryNormalizeIp(IPAddress? ipAddress, out string normalizedIp)
    {
        normalizedIp = string.Empty;
        if (ipAddress == null)
        {
            return false;
        }

        if (ipAddress.IsIPv4MappedToIPv6)
        {
            ipAddress = ipAddress.MapToIPv4();
        }

        normalizedIp = ipAddress.ToString();
        return true;
    }
}
