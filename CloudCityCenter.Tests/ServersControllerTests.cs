using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            new Server { Name = "A", Slug = "a", Location = "US", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = true },
            new Server { Name = "B", Slug = "b", Location = "EU", CpuCores = 8, RamGb = 32, StorageGb = 200, PricePerMonth = 50, IsActive = true },
            new Server { Name = "C", Slug = "c", Location = "US", CpuCores = 2, RamGb = 8, StorageGb = 100, PricePerMonth = 150, IsActive = false }
        );
        await context.SaveChangesAsync();

        var controller = new ServersController(context);

        var result = await controller.Index(location: "US", minRam: 10, maxRam: 64, q: null, sort: "price_asc", page: 1, pageSize: 12);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ServerCardVm>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("A", model[0].Name);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_ForMissingOrInactive()
    {
        var context = GetContext(nameof(Details_ReturnsNotFound_ForMissingOrInactive));
        context.Servers.AddRange(
            new Server { Name = "A", Slug = "a", Location = "US", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = true },
            new Server { Name = "C", Slug = "c", Location = "US", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 100, IsActive = false }
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
}

