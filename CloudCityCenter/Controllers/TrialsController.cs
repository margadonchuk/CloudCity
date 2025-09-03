using Microsoft.AspNetCore.Mvc;

namespace CloudCityCenter.Controllers;

/// <summary>
/// Handles the free trial flow for products.
/// </summary>
[Route("[controller]")]
public class TrialsController : Controller
{
    /// <summary>
    /// Starts a trial for the selected plan.
    /// </summary>
    [HttpGet("Start")]
    public IActionResult Start(string? plan)
    {
        if (string.IsNullOrWhiteSpace(plan))
        {
            return BadRequest();
        }

        return View(model: plan);
    }
}
