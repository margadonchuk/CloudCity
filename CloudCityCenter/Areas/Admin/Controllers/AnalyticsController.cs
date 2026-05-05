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

    public async Task<IActionResult> Index()
    {
        var visitors = await _context.VisitorSessions
            .AsNoTracking()
            .OrderByDescending(x => x.LastSeenAt)
            .Select(x => new VisitorSessionListItemViewModel
            {
                IpAddress = x.IpAddress,
                FirstSeenAt = x.FirstSeenAt,
                LastSeenAt = x.LastSeenAt
            })
            .ToListAsync();

        return View(new AnalyticsIndexViewModel
        {
            Visitors = visitors
        });
    }
}
