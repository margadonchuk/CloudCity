using System.Collections.Generic;
using System.Linq;
using CloudCityCenter.Models;

namespace CloudCityCenter.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Servers.Any())
        {
            return; // DB has been seeded
        }

        var servers = new List<Server>
        {
            new() { Name = "Starter US", Location = "US", PricePerMonth = 5, Configuration = "1 vCPU, 1GB RAM", IsAvailable = true },
            new() { Name = "Pro EU", Location = "EU", PricePerMonth = 15, Configuration = "2 vCPU, 4GB RAM", IsAvailable = true },
            new() { Name = "Enterprise Asia", Location = "Asia", PricePerMonth = 30, Configuration = "4 vCPU, 8GB RAM", IsAvailable = true }
        };

        context.Servers.AddRange(servers);
        context.SaveChanges();
    }
}
