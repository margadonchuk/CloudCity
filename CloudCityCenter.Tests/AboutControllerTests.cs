using CloudCityCenter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudCityCenter.Tests;

public class AboutControllerTests
{
    [Fact]
    public void Index_ReturnsViewResult()
    {
        var controller = new AboutController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
