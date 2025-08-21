using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace CloudCityCenter.Tests;

public class ServicesControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewResult_WithProducts()
    {
        var root = new InMemoryDatabaseRoot();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("Services_Index_ReturnsViewResult_WithProducts", root)
            .Options;
        await using var context = new ApplicationDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        context.Products.AddRange(
            new Product { Id = 1, Name = "S1", Location = "US", PricePerMonth = 10, Configuration = "C1", IsAvailable = true, ImageUrl = "img", Type = ProductType.DedicatedServer },
            new Product { Id = 2, Name = "S2", Location = "EU", PricePerMonth = 20, Configuration = "C2", IsAvailable = false, ImageUrl = "img", Type = ProductType.DedicatedServer }
        );
        await context.SaveChangesAsync();
        var controller = new ServicesController(context);

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ServiceIndexViewModel>(viewResult.Model);
        Assert.Equal(2, model.Products.Count());
        Assert.Contains(model.Products, s => s.Name == "S1");
        Assert.Contains(model.Products, s => s.Name == "S2");
    }
}
