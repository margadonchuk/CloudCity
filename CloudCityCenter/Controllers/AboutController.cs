using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
