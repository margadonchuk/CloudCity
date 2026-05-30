using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Tests;

public class ServiceControllerTests
{
    private static ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfServers()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(ServiceControllerTests) + nameof(Index_ReturnsViewResult_WithListOfServers));
        context.Servers.AddRange(
            new Server { Id = 1, Name = "Server1", Location = "US", PricePerMonth = 10, Configuration = "Conf1", IsAvailable = true, ImageUrl = "img" },
            new Server { Id = 2, Name = "Server2", Location = "EU", PricePerMonth = 20, Configuration = "Conf2", IsAvailable = true, ImageUrl = "img" }
        );
        await context.SaveChangesAsync();
        var controller = new ServiceController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Server>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }
}
