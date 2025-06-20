using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace CloudCityCenter.Tests;

public class ServiceControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewResult_WithServers()
    {
        var root = new InMemoryDatabaseRoot();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Service_Index_ReturnsViewResult_WithServers", root)
            .Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        context.Servers.AddRange(
            new Server { Id = 1, Name = "S1", Location = "US", PricePerMonth = 10, Configuration = "C1", IsAvailable = true, ImageUrl = "img" },
            new Server { Id = 2, Name = "S2", Location = "EU", PricePerMonth = 20, Configuration = "C2", IsAvailable = false, ImageUrl = "img" }
        );
        await context.SaveChangesAsync();
        var controller = new ServiceController(context);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ServiceIndexViewModel>(viewResult.Model);
        Assert.Equal(2, model.Servers.Count());
        Assert.Contains(model.Servers, s => s.Name == "S1");
        Assert.Contains(model.Servers, s => s.Name == "S2");
    }
}
