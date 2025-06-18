using System.Security.Claims;
using CloudCityCenter.Controllers;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Tests;

public class OrdersControllerTests
{
    private static ApplicationDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new ApplicationDbContext(options);
    }

    private static OrdersController GetController(ApplicationDbContext context, string userId)
    {
        var controller = new OrdersController(context);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "Test"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return controller;
    }

    [Fact]
    public async Task Index_ReturnsOrdersForCurrentUser()
    {
        var context = GetInMemoryDbContext(nameof(Index_ReturnsOrdersForCurrentUser));
        context.Servers.Add(new Server { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true });
        context.Orders.AddRange(
            new Order { Id = 1, UserId = "user1", ServerId = 1, TotalPrice = 5 },
            new Order { Id = 2, UserId = "user2", ServerId = 1, TotalPrice = 6 }
        );
        await context.SaveChangesAsync();
        var controller = GetController(context, "user1");

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Order>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal(1, model[0].Id);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        var context = GetInMemoryDbContext(nameof(Details_ReturnsNotFound_WhenOrderDoesNotExist));
        context.Servers.Add(new Server { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true });
        context.Orders.Add(new Order { Id = 1, UserId = "user1", ServerId = 1, TotalPrice = 5 });
        await context.SaveChangesAsync();
        var controller = GetController(context, "user1");

        var result = await controller.Details(2);

        Assert.IsType<NotFoundResult>(result);
    }
}
