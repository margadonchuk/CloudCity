using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Tests;

public class ServersControllerTests
{
    private static ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        var context = new ApplicationDbContext(options);
        return context;
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfServers()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(Index_ReturnsViewResult_WithListOfServers));
        context.Servers.AddRange(
            new Server { Id = 1, Name = "Server1", Location = "US", PricePerMonth = 10, Configuration = "Conf1", IsAvailable = true },
            new Server { Id = 2, Name = "Server2", Location = "EU", PricePerMonth = 20, Configuration = "Conf2", IsAvailable = true }
        );
        await context.SaveChangesAsync();
        var controller = new ServersController(context);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Server>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenIdIsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(Details_ReturnsNotFound_WhenIdIsNull));
        var controller = new ServersController(context);

        // Act
        var result = await controller.Details(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenServerDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(Details_ReturnsNotFound_WhenServerDoesNotExist));
        context.Servers.Add(new Server { Id = 1, Name = "Server1", Location = "US", PricePerMonth = 10, Configuration = "Conf1", IsAvailable = true });
        await context.SaveChangesAsync();
        var controller = new ServersController(context);

        // Act
        var result = await controller.Details(2);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
