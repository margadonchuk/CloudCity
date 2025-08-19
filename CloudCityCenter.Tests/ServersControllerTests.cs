using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Collections.Generic;

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

    private static ServersController GetController(ApplicationDbContext context, bool authenticated = true)
    {
        var controller = new ServersController(context);
        var httpContext = new DefaultHttpContext();
        if (authenticated)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "user1"), new Claim(ClaimTypes.Role, "Admin") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
            httpContext.User = user;
        }
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsOnlyPublishedDedicatedServers()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(Index_ReturnsOnlyPublishedDedicatedServers));
        context.Products.AddRange(
            new Product { Id = 1, Name = "Server1", Slug = "s1", Location = "US", PricePerMonth = 10, Configuration = "Conf1", IsAvailable = true, ImageUrl = "img", Type = ProductType.DedicatedServer, IsPublished = true },
            new Product { Id = 2, Name = "Server2", Slug = "s2", Location = "EU", PricePerMonth = 20, Configuration = "Conf2", IsAvailable = true, ImageUrl = "img", Type = ProductType.DedicatedServer, IsPublished = false },
            new Product { Id = 3, Name = "Hosting", Slug = "h1", Location = "EU", PricePerMonth = 5, Configuration = "Conf3", IsAvailable = true, ImageUrl = "img", Type = ProductType.Hosting, IsPublished = true }
        );
        await context.SaveChangesAsync();
        var controller = GetController(context, authenticated: false);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ProductCardVm>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("Server1", model[0].Name);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenSlugIsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext(nameof(Details_ReturnsNotFound_WhenSlugIsNull));
        var controller = GetController(context, authenticated: false);

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
        context.Products.Add(new Product { Id = 1, Name = "Server1", Slug = "s1", Location = "US", PricePerMonth = 10, Configuration = "Conf1", IsAvailable = true, ImageUrl = "img", Type = ProductType.DedicatedServer, IsPublished = true });
        await context.SaveChangesAsync();
        var controller = GetController(context);

        // Act
        var result = await controller.Details("nope");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewResult_WithServer()
    {
        // Arrange
        var dbName = nameof(Details_ReturnsViewResult_WithServer);
        var seedContext = GetInMemoryDbContext(dbName);
        var server = new Product
        {
            Id = 1,
            Name = "Server1",
            Slug = "s1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true,
            ImageUrl = "img",
            Type = ProductType.DedicatedServer,
            IsPublished = true,
            Variants = new List<ProductVariant>
            {
                new ProductVariant { Id = 1, Name = "Basic", Price = 5, BillingPeriod = BillingPeriod.Monthly }
            },
            Features = new List<ProductFeature>
            {
                new ProductFeature { Id = 1, Name = "RAM", Value = "16GB" }
            }
        };
        seedContext.Products.Add(server);
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName), authenticated: false);

        // Act
        var result = await controller.Details("s1");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Product>(viewResult.Model);
        Assert.Equal("Server1", model.Name);
        Assert.Single(model.Variants);
        Assert.Single(model.Features);
    }

    [Fact]
    public async Task Create_AddsServerAndRedirects_WhenModelStateValid()
    {
        // Arrange
        var dbName = nameof(Create_AddsServerAndRedirects_WhenModelStateValid);
        var context = GetInMemoryDbContext(dbName);
        var controller = GetController(context);
        var server = new Product
        {
            Name = "NewServer",
            Location = "EU",
            PricePerMonth = 5,
            Configuration = "Conf",
            IsAvailable = true,
            ImageUrl = "img",
            Type = ProductType.DedicatedServer
        };

        // Act
        var result = await controller.Create(server);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var created = await verifyContext.Products.FirstOrDefaultAsync();
        Assert.NotNull(created);
        Assert.Equal("NewServer", created!.Name);
    }

    [Fact]
    public async Task Edit_UpdatesServerAndRedirects_WhenModelStateValid()
    {
        // Arrange
        var dbName = nameof(Edit_UpdatesServerAndRedirects_WhenModelStateValid);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Products.Add(new Product
        {
            Id = 1,
            Name = "Server1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true,
            ImageUrl = "img",
            Type = ProductType.DedicatedServer
        });
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName));
        var updatedServer = new Product
        {
            Id = 1,
            Name = "Updated",
            Location = "US",
            PricePerMonth = 20,
            Configuration = "Conf2",
            IsAvailable = false,
            ImageUrl = "img",
            Type = ProductType.DedicatedServer
        };

        // Act
        var result = await controller.Edit(1, updatedServer);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var serverInDb = await verifyContext.Products.FirstAsync();
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
        seedContext.Products.Add(new Product
        {
            Id = 1,
            Name = "Server1",
            Location = "US",
            PricePerMonth = 10,
            Configuration = "Conf1",
            IsAvailable = true,
            ImageUrl = "img",
            Type = ProductType.DedicatedServer
        });
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName));

        // Act
        var result = await controller.DeleteConfirmed(1);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        Assert.Empty(verifyContext.Products);
    }

}
