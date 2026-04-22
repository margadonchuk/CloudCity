using System.Net;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

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

    [HttpGet]
    [Route("admin/blockedips")]
    [Route("admin/security/blockedips")]
    public async Task<IActionResult> Index()
    {
        List<BlockedIp> blockedIps;
        try
        {
            blockedIps = await _context.BlockedIps
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            TempData["ErrorMessage"] = "Blocked IP list is temporarily unavailable because the BlockedIps table schema is out of sync.";
            blockedIps = new List<BlockedIp>();
        }

        return View(blockedIps);
    }

    public IActionResult Create()
    {
        return View(new BlockedIpCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlockedIpCreateViewModel model, bool returnToIndex = false)
    {
        if (!ModelState.IsValid)
        {
            if (returnToIndex)
            {
                TempData["ErrorMessage"] = "Invalid IP address";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        if (!TryNormalizeIp(model.IpAddress, out var normalizedIp))
        {
            ModelState.AddModelError(nameof(model.IpAddress), "Please enter a valid IPv4 or IPv6 address.");
            if (returnToIndex)
            {
                TempData["ErrorMessage"] = "Invalid IP address";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        bool alreadyBlocked;
        try
        {
            alreadyBlocked = await _context.BlockedIps
                .AnyAsync(x => x.IsActive && x.IpAddress == normalizedIp);
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            TempData["ErrorMessage"] = "Blocked IP list is temporarily unavailable because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }

        if (alreadyBlocked)
        {
            TempData["ErrorMessage"] = "IP address already exists";
            ModelState.AddModelError(nameof(model.IpAddress), "This IP address is already blocked.");
            if (returnToIndex)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        var entity = new BlockedIp
        {
            IpAddress = normalizedIp,
            Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.BlockedIps.Add(entity);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            TempData["ErrorMessage"] = "Could not block IP because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "IP address blocked successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unblock(int id)
    {
        BlockedIp? entry;
        try
        {
            entry = await _context.BlockedIps.FindAsync(id);
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            TempData["ErrorMessage"] = "Could not update blocked IP because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }
        if (entry == null)
        {
            TempData["ErrorMessage"] = "Blocked IP was not found";
            return RedirectToAction(nameof(Index));
        }

        entry.IsActive = false;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            TempData["ErrorMessage"] = "Could not update blocked IP because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }

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

    private static bool IsBlockedIpDataUnavailable(Exception exception) =>
        exception is DbUpdateException
            or InvalidOperationException
            or DbException;
}
