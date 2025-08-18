using Microsoft.AspNetCore.Mvc;

public class HostingController : Controller
{
    public IActionResult Index() => View();
}
