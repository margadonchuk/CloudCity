using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

[AllowAnonymous]
public class CoinController : Controller
{
    private readonly IStringLocalizerFactory _localizerFactory;

    public CoinController(IStringLocalizerFactory localizerFactory)
    {
        _localizerFactory = localizerFactory;
    }

    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.Coin.Index", "CloudCityCenter");
    }

    [HttpGet("coin")]
    [HttpGet("cloudcity-coin")]
    public IActionResult Index()
    {
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;

        return View();
    }
}
