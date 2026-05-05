using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
