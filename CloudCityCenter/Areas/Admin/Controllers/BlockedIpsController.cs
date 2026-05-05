using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.Admin;
using CloudCityCenter.Services;
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
    private readonly ILogger<BlockedIpsController> _logger;

    public BlockedIpsController(ApplicationDbContext context, ILogger<BlockedIpsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("admin/blockedips")]
    [Route("admin/security/blockedips")]
    public async Task<IActionResult> Index()
    {
        var requestPath = HttpContext.Request.Path.Value ?? "/admin/security/blockedips";
        var userName = User?.Identity?.Name ?? "<anonymous>";


        try
        {
            var blockedIps = await _context.BlockedIps
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new BlockedIpListItemViewModel
                {
                    Id = x.Id,
                    IpAddress = x.IpAddress ?? "(unknown)",
                    Reason = x.Reason,
                    CreatedAtUtc = x.CreatedAt,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            return View(new BlockedIpsIndexViewModel
            {
                Items = blockedIps
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "BlockedIps index failed for {Path}. User={UserName}. Message={Message}. StackTrace={StackTrace}. Inner={InnerMessage}",
                requestPath,
                userName,
                ex.Message,
                ex.StackTrace,
                ex.InnerException?.Message);

            return View(new BlockedIpsIndexViewModel
            {
                IsFeatureInitialized = false,
                FeatureMessage = "Blocked IP list is temporarily unavailable.",
                Items = Array.Empty<BlockedIpListItemViewModel>()
            });
        }
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

        if (!ClientIpResolver.TryNormalizeIp(model.IpAddress, out var normalizedIp))
        {
            ModelState.AddModelError(nameof(model.IpAddress), "Please enter a valid IPv4 or IPv6 address.");
            if (returnToIndex)
            {
                TempData["ErrorMessage"] = "Invalid IP address";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        BlockedIp? existingEntry;
        try
        {
            existingEntry = await _context.BlockedIps
                .FirstOrDefaultAsync(x => x.IpAddress == normalizedIp);
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            _logger.LogWarning(ex,
                "Could not validate blocked IP uniqueness because blocked IP data is unavailable.");
            TempData["ErrorMessage"] = "Blocked IP list is temporarily unavailable because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }

        if (existingEntry?.IsActive == true)
        {
            TempData["ErrorMessage"] = "IP address already exists";
            ModelState.AddModelError(nameof(model.IpAddress), "This IP address is already blocked.");
            if (returnToIndex)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        var reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim();
        if (existingEntry is not null)
        {
            existingEntry.IpAddress = normalizedIp;
            existingEntry.Reason = reason;
            existingEntry.CreatedAt = DateTime.UtcNow;
            existingEntry.IsActive = true;
        }
        else
        {
            var entity = new BlockedIp
            {
                IpAddress = normalizedIp,
                Reason = reason,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.BlockedIps.Add(entity);
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            _logger.LogWarning(ex,
                "Could not save blocked IP entry because blocked IP data is unavailable.");
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
            entry = await _context.BlockedIps.FirstOrDefaultAsync(x => x.Id == id);
        }
        catch (Exception ex) when (IsBlockedIpDataUnavailable(ex))
        {
            _logger.LogWarning(ex,
                "Could not load blocked IP entry {BlockedIpId} because blocked IP data is unavailable.",
                id);
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
            _logger.LogWarning(ex,
                "Could not update blocked IP entry {BlockedIpId} because blocked IP data is unavailable.",
                id);
            TempData["ErrorMessage"] = "Could not update blocked IP because the BlockedIps table schema is out of sync.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "IP address removed from block list";
        return RedirectToAction(nameof(Index));
    }

    private static bool IsBlockedIpDataUnavailable(Exception exception) =>
        exception is DbUpdateException
            or DbException
            or InvalidOperationException
            or InvalidCastException
            or FormatException
            or OverflowException
            or NullReferenceException
        || (exception.InnerException is not null && IsBlockedIpDataUnavailable(exception.InnerException));
}
