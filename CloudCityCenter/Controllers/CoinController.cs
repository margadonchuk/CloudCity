using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Controllers;

[AllowAnonymous]
public class CoinController : Controller
{
    [HttpGet("coin")]
    [HttpGet("cloudcity-coin")]
    public IActionResult Index()
    {
        ViewData["Title"] = "CloudCity Coin";
        ViewData["Description"] = "CloudCity Coin is the digital coin of the CloudCity ecosystem and a planned additional payment option for selected CloudCity services.";
        ViewData["Keywords"] = "CloudCity Coin, CloudCity crypto, Solana coin, CloudCity payment option";

        return View();
    }
}
