using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Controllers;

public class ContactController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
