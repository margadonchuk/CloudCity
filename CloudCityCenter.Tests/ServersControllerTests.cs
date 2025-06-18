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

    [Fact]
    public async Task Details_ReturnsViewResult_WithServer()
    {
        // Arrange
        var dbName = nameof(Details_ReturnsViewResult_WithServer);
        var seedContext = GetInMemoryDbContext(dbName);
        var server = new Server
        {
            Id = 1,
            Name = "Server1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true
        };
        seedContext.Servers.Add(server);
        await seedContext.SaveChangesAsync();

        var controller = new ServersController(GetInMemoryDbContext(dbName));

        // Act
        var result = await controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Server>(viewResult.Model);
        Assert.Equal("Server1", model.Name);
    }

    [Fact]
    public async Task Create_AddsServerAndRedirects_WhenModelStateValid()
    {
        // Arrange
        var dbName = nameof(Create_AddsServerAndRedirects_WhenModelStateValid);
        var context = GetInMemoryDbContext(dbName);
        var controller = new ServersController(context);
        var server = new Server
        {
            Name = "NewServer",
            Location = "EU",
            PricePerMonth = 5,
            Configuration = "Conf",
            IsAvailable = true
        };

        // Act
        var result = await controller.Create(server);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var created = await verifyContext.Servers.FirstOrDefaultAsync();
        Assert.NotNull(created);
        Assert.Equal("NewServer", created!.Name);
    }

    [Fact]
    public async Task Edit_UpdatesServerAndRedirects_WhenModelStateValid()
    {
        // Arrange
        var dbName = nameof(Edit_UpdatesServerAndRedirects_WhenModelStateValid);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Servers.Add(new Server
        {
            Id = 1,
            Name = "Server1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true
        });
        await seedContext.SaveChangesAsync();

        var controller = new ServersController(GetInMemoryDbContext(dbName));
        var updatedServer = new Server
        {
            Id = 1,
            Name = "Updated",
            Location = "US",
            PricePerMonth = 20,
            Configuration = "Conf2",
            IsAvailable = false
        };

        // Act
        var result = await controller.Edit(1, updatedServer);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var serverInDb = await verifyContext.Servers.FirstAsync();
        Assert.Equal("Updated", serverInDb.Name);
        Assert.Equal(20, serverInDb.PricePerMonth);
        Assert.False(serverInDb.IsAvailable);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesServerAndRedirects()
    {
        // Arrange
        var dbName = nameof(DeleteConfirmed_RemovesServerAndRedirects);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Servers.Add(new Server
        {
            Id = 1,
            Name = "Server1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true
        });
        await seedContext.SaveChangesAsync();

        var controller = new ServersController(GetInMemoryDbContext(dbName));

        // Act
        var result = await controller.DeleteConfirmed(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        Assert.Empty(verifyContext.Servers);
    }
}
