using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace CloudCityCenter.Tests;

public class ServersAuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ServersAuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("AuthTests"));
            });
        });
    }

    [Theory]
    [InlineData("/Servers/Create")]
    [InlineData("/Servers/Edit/1")]
    [InlineData("/Servers/Delete/1")]
    public async Task Get_SecuredRoutes_RedirectUnauthenticated(string url)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public void Controller_HasAuthorizeAttributeWithAdminRole()
    {
        var attr = (AuthorizeAttribute?)Attribute.GetCustomAttribute(typeof(ServersController), typeof(AuthorizeAttribute));
        Assert.NotNull(attr);
        Assert.Equal("Admin", attr!.Roles);
    }

    [Fact]
    public void IndexAndDetails_AreAllowAnonymous()
    {
        var indexAttr = typeof(ServersController).GetMethod(nameof(ServersController.Index))
            ?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
            .FirstOrDefault();
        var detailsAttr = typeof(ServersController).GetMethod(nameof(ServersController.Details))
            ?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false)
            .FirstOrDefault();
        Assert.NotNull(indexAttr);
        Assert.NotNull(detailsAttr);
    }
}
