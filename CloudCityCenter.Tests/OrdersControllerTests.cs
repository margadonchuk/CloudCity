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
        context.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        context.Orders.AddRange(
            new Order { Id = 1, UserId = "user1", ProductId = 1, TotalPrice = 5, Status = OrderStatus.Pending },
            new Order { Id = 2, UserId = "user2", ProductId = 1, TotalPrice = 6, Status = OrderStatus.Completed }
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
        context.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        context.Orders.Add(new Order { Id = 1, UserId = "user1", ProductId = 1, TotalPrice = 5 });
        await context.SaveChangesAsync();
        var controller = GetController(context, "user1");

        var result = await controller.Details(2);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewResult_WithOrderForCurrentUser()
    {
        var dbName = nameof(Details_ReturnsViewResult_WithOrderForCurrentUser);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        seedContext.Orders.Add(new Order { Id = 1, UserId = "user1", ProductId = 1, TotalPrice = 5, Status = OrderStatus.Pending });
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName), "user1");

        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Order>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Create_PersistsOrderAndRedirectsToIndex()
    {
        var dbName = nameof(Create_PersistsOrderAndRedirectsToIndex);
        var context = GetInMemoryDbContext(dbName);
        context.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        await context.SaveChangesAsync();
        var controller = GetController(context, "user1");
        var order = new Order { ProductId = 1, TotalPrice = 10, Status = OrderStatus.Completed };

        var result = await controller.Create(order);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var created = await verifyContext.Orders.FirstOrDefaultAsync();
        Assert.NotNull(created);
        Assert.Equal(1, created!.ProductId);
        Assert.Equal("user1", created.UserId);
        Assert.Equal(OrderStatus.Completed, created.Status);
    }

    [Fact]
    public async Task Edit_UpdatesOrderAndRedirectsToIndex()
    {
        var dbName = nameof(Edit_UpdatesOrderAndRedirectsToIndex);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        seedContext.Orders.Add(new Order { Id = 1, UserId = "user1", ProductId = 1, TotalPrice = 5, Status = OrderStatus.Pending });
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName), "user1");
        var updated = new Order { Id = 1, ProductId = 1, TotalPrice = 20, Status = OrderStatus.Canceled };

        var result = await controller.Edit(1, updated);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        var orderInDb = await verifyContext.Orders.FirstAsync();
        Assert.Equal(20, orderInDb.TotalPrice);
        Assert.Equal(OrderStatus.Canceled, orderInDb.Status);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesOrder()
    {
        var dbName = nameof(DeleteConfirmed_RemovesOrder);
        var seedContext = GetInMemoryDbContext(dbName);
        seedContext.Products.Add(new Product { Id = 1, Name = "S1", Location = "L", PricePerMonth = 1, Configuration = "C", IsAvailable = true, Type = ProductType.DedicatedServer });
        seedContext.Orders.Add(new Order { Id = 1, UserId = "user1", ProductId = 1, TotalPrice = 5, Status = OrderStatus.Pending });
        await seedContext.SaveChangesAsync();

        var controller = GetController(GetInMemoryDbContext(dbName), "user1");

        var result = await controller.DeleteConfirmed(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        using var verifyContext = GetInMemoryDbContext(dbName);
        Assert.Empty(verifyContext.Orders);
    }
}
