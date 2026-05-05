using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models.Admin;
using CloudCityCenter.Models;
using CloudCityCenter.Services;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnalyticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? range = "today", DateTime? startDate = null, DateTime? endDate = null, string? ipAddress = null)
    {
        var selectedRange = (range ?? "today").Trim().ToLowerInvariant();
        var searchIpAddress = (ipAddress ?? string.Empty).Trim();
        var utcToday = DateTime.UtcNow.Date;

        DateTime filterStart;
        DateTime filterEndExclusive;

        if (selectedRange == "custom")
        {
            var resolvedStart = (startDate ?? utcToday).Date;
            var resolvedEnd = (endDate ?? resolvedStart).Date;

            if (resolvedEnd < resolvedStart)
            {
                (resolvedStart, resolvedEnd) = (resolvedEnd, resolvedStart);
            }

            filterStart = resolvedStart;
            filterEndExclusive = resolvedEnd.AddDays(1);
        }
        else
        {
            switch (selectedRange)
            {
                case "last7days":
                    filterStart = utcToday.AddDays(-6);
                    break;
                case "last30days":
                    filterStart = utcToday.AddDays(-29);
                    break;
                default:
                    selectedRange = "today";
                    filterStart = utcToday;
                    break;
            }

            filterEndExclusive = utcToday.AddDays(1);
        }

        var visitorSessionsQuery = _context.VisitorSessions
            .AsNoTracking()
            .Where(x => x.LastSeenAt >= filterStart && x.LastSeenAt < filterEndExclusive);

        if (!string.IsNullOrWhiteSpace(searchIpAddress))
        {
            visitorSessionsQuery = visitorSessionsQuery
                .Where(x => x.IpAddress.Contains(searchIpAddress));
        }

        var visitorSessionRows = await visitorSessionsQuery
            .OrderByDescending(x => x.LastSeenAt)
            .Select(x => new
            {
                x.Id,
                x.IpAddress,
                x.FirstSeenAt,
                x.LastSeenAt,
                PagesCount = x.PageVisits.Count,
                x.UserAgent
            })
            .ToListAsync();

        var visitors = visitorSessionRows
            .Select(x =>
            {
                var hasNormalizedIp = ClientIpResolver.TryNormalizeIp(x.IpAddress, out var normalizedIp);
                var browser = DetectBrowser(x.UserAgent);
                var device = DetectDevice(x.UserAgent);
                return new VisitorSessionListItemViewModel
                {
                    Id = x.Id,
                    IpAddress = x.IpAddress,
                    NormalizedIpAddress = hasNormalizedIp ? normalizedIp : string.Empty,
                    FirstSeenAt = x.FirstSeenAt,
                    LastSeenAt = x.LastSeenAt,
                    PagesCount = x.PagesCount,
                    UserAgent = x.UserAgent,
                    Browser = browser,
                    Device = device
                };
            })
            .ToList();

        var topBrowsers = visitors
            .GroupBy(v => v.Browser)
            .Select(g => new AnalyticsBreakdownItemViewModel { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Label)
            .ToList();

        var deviceSplit = visitors
            .GroupBy(v => v.Device)
            .Select(g => new AnalyticsBreakdownItemViewModel { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Label)
            .ToList();

        var normalizedIps = visitors
            .Select(x => x.NormalizedIpAddress)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        var blockedIpSet = new HashSet<string>(StringComparer.Ordinal);
        if (normalizedIps.Count != 0)
        {
            var blockedIpList = await _context.BlockedIps
                .AsNoTracking()
                .Where(x => x.IsActive && normalizedIps.Contains(x.IpAddress))
                .Select(x => x.IpAddress)
                .ToListAsync();

            blockedIpSet = blockedIpList.ToHashSet(StringComparer.Ordinal);
        }

        visitors = visitors
            .Select(x => new VisitorSessionListItemViewModel
            {
                Id = x.Id,
                IpAddress = x.IpAddress,
                NormalizedIpAddress = x.NormalizedIpAddress,
                FirstSeenAt = x.FirstSeenAt,
                LastSeenAt = x.LastSeenAt,
                PagesCount = x.PagesCount,
                UserAgent = x.UserAgent,
                Browser = x.Browser,
                Device = x.Device,
                IsIpBlocked = !string.IsNullOrWhiteSpace(x.NormalizedIpAddress) && blockedIpSet.Contains(x.NormalizedIpAddress)
            })
            .ToList();

        var topPages = await _context.PageVisits
            .AsNoTracking()
            .Where(x => x.VisitedAt >= filterStart && x.VisitedAt < filterEndExclusive)
            .Where(x => !x.Path.StartsWith("/Admin/Analytics"))
            .GroupBy(x => x.Path)
            .Select(x => new TopPageVisitViewModel
            {
                Path = x.Key,
                VisitsCount = x.Count(),
                LastVisitAt = x.Max(p => p.VisitedAt)
            })
            .OrderByDescending(x => x.VisitsCount)
            .ThenBy(x => x.Path)
            .Take(10)
            .ToListAsync();

        var totalPageVisits = await _context.PageVisits
            .AsNoTracking()
            .Where(x => x.VisitedAt >= filterStart && x.VisitedAt < filterEndExclusive)
            .CountAsync();

        var onlineThresholdUtc = DateTime.UtcNow.AddMinutes(-5);
        var activeVisitorRows = await _context.VisitorSessions
            .AsNoTracking()
            .Where(x => x.LastSeenAt >= onlineThresholdUtc)
            .OrderByDescending(x => x.LastSeenAt)
            .Select(x => new
            {
                x.Id,
                x.IpAddress,
                x.FirstSeenAt,
                x.LastSeenAt,
                PagesCount = x.PageVisits.Count,
                x.UserAgent
            })
            .ToListAsync();

        var activeVisitors = activeVisitorRows
            .Select(x =>
            {
                var hasNormalizedIp = ClientIpResolver.TryNormalizeIp(x.IpAddress, out var normalizedIp);
                return new VisitorSessionListItemViewModel
                {
                    Id = x.Id,
                    IpAddress = x.IpAddress,
                    NormalizedIpAddress = hasNormalizedIp ? normalizedIp : string.Empty,
                    FirstSeenAt = x.FirstSeenAt,
                    LastSeenAt = x.LastSeenAt,
                    PagesCount = x.PagesCount,
                    UserAgent = x.UserAgent,
                    Browser = DetectBrowser(x.UserAgent),
                    Device = DetectDevice(x.UserAgent)
                };
            })
            .ToList();

        return View(new AnalyticsIndexViewModel
        {
            SelectedFilter = selectedRange,
            SearchIpAddress = searchIpAddress,
            StartDateUtc = filterStart,
            EndDateUtc = filterEndExclusive.AddTicks(-1),
            TotalPageVisits = totalPageVisits,
            OnlineNowCount = activeVisitors.Count,
            ActiveVisitors = activeVisitors,
            Visitors = visitors,
            TopPages = topPages,
            TopBrowsers = topBrowsers,
            DeviceSplit = deviceSplit
        });
    }

    private static string DetectBrowser(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "Other";
        }

        if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase)) return "Edge";
        if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase)) return "Firefox";
        if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase) &&
            !userAgent.Contains("Chromium", StringComparison.OrdinalIgnoreCase)) return "Chrome";
        if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase) &&
            !userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase) &&
            !userAgent.Contains("Chromium", StringComparison.OrdinalIgnoreCase)) return "Safari";

        return "Other";
    }

    private static string DetectDevice(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "Desktop";
        }

        if (userAgent.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("crawl", StringComparison.OrdinalIgnoreCase)) return "Bot";
        if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase)) return "Tablet";
        if (userAgent.Contains("Mobi", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase)) return "Mobile";

        return "Desktop";
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _context.VisitorSessions
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AnalyticsDetailsViewModel
            {
                VisitorSessionId = x.Id,
                IpAddress = x.IpAddress,
                FirstSeenAt = x.FirstSeenAt,
                LastSeenAt = x.LastSeenAt,
                UserAgent = x.UserAgent,
                Referrer = x.Referrer,
                Visits = x.PageVisits
                    .OrderByDescending(p => p.VisitedAt)
                    .Select(p => new PageVisitTimelineItemViewModel
                    {
                        VisitedAt = p.VisitedAt,
                        HttpMethod = p.HttpMethod,
                        Path = p.Path,
                        QueryString = p.QueryString
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (session is null)
        {
            return NotFound();
        }

        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BlockIp(int visitorSessionId, string? range = "today", DateTime? startDate = null, DateTime? endDate = null, string? ipAddress = null)
    {
        var session = await _context.VisitorSessions
            .AsNoTracking()
            .Where(x => x.Id == visitorSessionId)
            .Select(x => new { x.IpAddress })
            .FirstOrDefaultAsync();

        if (session is null || !ClientIpResolver.TryNormalizeIp(session.IpAddress, out var normalizedIp))
        {
            TempData["ErrorMessage"] = "Could not resolve IP address from this visitor session.";
            return RedirectToAction(nameof(Index), new { range, startDate, endDate, ipAddress });
        }

        var currentAdminIp = ClientIpResolver.ResolveNormalizedIp(HttpContext);
        var confirmSelfBlock = string.Equals(Request.Form["confirmSelfBlock"], "true", StringComparison.OrdinalIgnoreCase);
        if (string.Equals(currentAdminIp, normalizedIp, StringComparison.Ordinal) && !confirmSelfBlock)
        {
            TempData["ErrorMessage"] = "You are trying to block your current admin IP. Submit again with confirmation.";
            return RedirectToAction(nameof(Index), new { range, startDate, endDate, ipAddress });
        }

        var existingEntry = await _context.BlockedIps.FirstOrDefaultAsync(x => x.IpAddress == normalizedIp);
        if (existingEntry?.IsActive == true)
        {
            TempData["InfoMessage"] = "Already blocked";
            return RedirectToAction(nameof(Index), new { range, startDate, endDate, ipAddress });
        }

        if (existingEntry is null)
        {
            _context.BlockedIps.Add(new BlockedIp
            {
                IpAddress = normalizedIp,
                Reason = "Blocked from Analytics",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }
        else
        {
            existingEntry.IsActive = true;
            existingEntry.CreatedAt = DateTime.UtcNow;
            existingEntry.Reason = string.IsNullOrWhiteSpace(existingEntry.Reason) ? "Blocked from Analytics" : existingEntry.Reason;
        }

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "IP address blocked successfully";
        return RedirectToAction(nameof(Index), new { range, startDate, endDate, ipAddress });
    }
}
