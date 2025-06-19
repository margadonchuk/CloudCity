using CloudCityCenter.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace CloudCityCenter.Tests;

public class ContactControllerTests
{
    [Fact]
    public void Index_ReturnsViewResult()
    {
        var controller = new ContactController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
