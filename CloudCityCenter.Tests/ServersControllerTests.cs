using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CloudCityCenter.Tests;

public class ServersControllerTests
{
    private static ApplicationDbContext GetContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task Index_FiltersSortsAndPaginates()
    {
        var context = GetContext(nameof(Index_FiltersSortsAndPaginates));
        context.Servers.AddRange(
            new Server { Name = "A", Slug = "a", Location = "US", CPU = "4 cores", RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = true },
            new Server { Name = "B", Slug = "b", Location = "EU", CPU = "8 cores", RamGb = 32, StorageGb = 200, PricePerMonth = 50, IsActive = true },
            new Server { Name = "C", Slug = "c", Location = "US", CPU = "2 cores", RamGb = 8, StorageGb = 100, PricePerMonth = 150, IsActive = false }
        );
        await context.SaveChangesAsync();

        var controller = new ServersController(context);

        var result = await controller.Index(location: "US", minRam: 10, maxRam: 64, q: null, sort: "price_asc", page: 1, pageSize: 12);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ServerIndexViewModel>(viewResult.Model);
        Assert.Single(model.Servers);
        Assert.Equal("A", model.Servers.First().Name);
    }

    [Fact]
    public async Task Index_SearchesByCpuAndLocation()
    {
        var context = GetContext(nameof(Index_SearchesByCpuAndLocation));
        context.Servers.AddRange(
            new Server { Name = "Xeon", Slug = "xeon", Location = "US", CPU = "Xeon", RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = true },
            new Server { Name = "Ryzen", Slug = "ryzen", Location = "EU", CPU = "Ryzen", RamGb = 32, StorageGb = 200, PricePerMonth = 50, IsActive = true }
        );
        await context.SaveChangesAsync();

        var controller = new ServersController(context);

        var cpuResult = await controller.Index(location: null, minRam: null, maxRam: null, q: "Xeon", sort: null, page: 1, pageSize: 12);
        var cpuModel = Assert.IsAssignableFrom<ServerIndexViewModel>(Assert.IsType<ViewResult>(cpuResult).Model);
        Assert.Single(cpuModel.Servers);
        Assert.Equal("Xeon", cpuModel.Servers.First().Name);

        var locationResult = await controller.Index(location: null, minRam: null, maxRam: null, q: "EU", sort: null, page: 1, pageSize: 12);
        var locationModel = Assert.IsAssignableFrom<ServerIndexViewModel>(Assert.IsType<ViewResult>(locationResult).Model);
        Assert.Single(locationModel.Servers);
        Assert.Equal("Ryzen", locationModel.Servers.First().Name);
    }

    [Fact]
    public async Task Index_Search_IsCaseInsensitive()
    {
        var context = GetContext(nameof(Index_Search_IsCaseInsensitive));
        context.Servers.Add(new Server
        {
            Name = "Alpha",
            Slug = "alpha",
            Location = "US",
            CPU = "4 cores",
            RamGb = 16,
            StorageGb = 100,
            PricePerMonth = 100,
            IsActive = true
        });
        await context.SaveChangesAsync();

        var controller = new ServersController(context);

        var lowerResult = await controller.Index(location: null, minRam: null, maxRam: null, q: "alpha", sort: null, page: 1, pageSize: 12);
        var lowerModel = Assert.IsAssignableFrom<ServerIndexViewModel>(Assert.IsType<ViewResult>(lowerResult).Model);
        Assert.Single(lowerModel.Servers);

        var upperResult = await controller.Index(location: null, minRam: null, maxRam: null, q: "ALPHA", sort: null, page: 1, pageSize: 12);
        var upperModel = Assert.IsAssignableFrom<ServerIndexViewModel>(Assert.IsType<ViewResult>(upperResult).Model);
        Assert.Single(upperModel.Servers);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_ForMissingOrInactive()
    {
        var context = GetContext(nameof(Details_ReturnsNotFound_ForMissingOrInactive));
        context.Servers.AddRange(
            new Server { Name = "A", Slug = "a", Location = "US", CPU = "4 cores", RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = true },
            new Server { Name = "C", Slug = "c", Location = "US", CPU = "4 cores", RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = false }
        );
        await context.SaveChangesAsync();

        var controller = new ServersController(context);

        Assert.IsType<NotFoundResult>(await controller.Details("missing"));
        Assert.IsType<NotFoundResult>(await controller.Details("c"));

        var okResult = await controller.Details("a");
        var viewResult = Assert.IsType<ViewResult>(okResult);
        var model = Assert.IsType<Server>(viewResult.Model);
        Assert.Equal("A", model.Name);
    }

    [Fact]
    public async Task Get_Server_BySlug_ReturnsDetailsView()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Servers.Add(new Server
        {
            Name = "Alpha",
            Slug = "alpha",
            Location = "US",
            CPU = "4 cores",
            RamGb = 16,
            StorageGb = 100,
            PricePerMonth = 10,
            IsActive = true
        });
        db.SaveChanges();

        var response = await client.GetAsync("/Servers/alpha");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Alpha", content);
    }
}

