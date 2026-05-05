using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models.Admin;

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

        var visitors = await visitorSessionsQuery
            .OrderByDescending(x => x.LastSeenAt)
            .Select(x => new VisitorSessionListItemViewModel
            {
                Id = x.Id,
                IpAddress = x.IpAddress,
                FirstSeenAt = x.FirstSeenAt,
                LastSeenAt = x.LastSeenAt,
                PagesCount = x.PageVisits.Count,
                UserAgent = x.UserAgent
            })
            .ToListAsync();

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

        return View(new AnalyticsIndexViewModel
        {
            SelectedFilter = selectedRange,
            SearchIpAddress = searchIpAddress,
            StartDateUtc = filterStart,
            EndDateUtc = filterEndExclusive.AddTicks(-1),
            TotalPageVisits = totalPageVisits,
            Visitors = visitors,
            TopPages = topPages
        });
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
}
