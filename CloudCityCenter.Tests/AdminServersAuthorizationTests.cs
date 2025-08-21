using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CloudCityCenter.Tests;

public class AdminServersAuthorizationTests
{
    [Fact]
    public async Task Get_AdminServers_Unauthenticated_RedirectsToLogin()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/Admin/Servers");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task Get_AdminServers_NonAdmin_Forbidden()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-User", "user");

        var response = await client.GetAsync("/Admin/Servers");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Get_AdminServers_Admin_Ok()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-User", "admin");
        client.DefaultRequestHeaders.Add("X-Role", "Admin");

        var response = await client.GetAsync("/Admin/Servers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

