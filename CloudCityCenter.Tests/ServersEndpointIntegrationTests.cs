using CloudCityCenter.Data;
using CloudCityCenter.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace CloudCityCenter.Tests;

public class ServersEndpointIntegrationTests
{
    [Fact]
    public async Task Get_Servers_OnlyActiveAndFiltered()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Servers.AddRange(
            new Server { Name = "ActiveUS", Slug = "a", Location = "US", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 10, IsActive = true },
            new Server { Name = "InactiveUS", Slug = "b", Location = "US", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 10, IsActive = false },
            new Server { Name = "ActiveEU", Slug = "c", Location = "EU", CpuCores = 4, RamGb = 16, StorageGb = 100, PricePerMonth = 10, IsActive = true }
        );
        db.SaveChanges();

        var response = await client.GetAsync("/Servers?location=US");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("ActiveUS", content);
        Assert.DoesNotContain("InactiveUS", content);
        Assert.DoesNotContain("ActiveEU", content);
    }
}

