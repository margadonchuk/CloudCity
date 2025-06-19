using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using CloudCityCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.Migrate();

        if (!context.Servers.Any())
        {
            var servers = new List<Server>
            {
                new()
                {
                    Name = "Starter US",
                    Location = "US",
                    PricePerMonth = 5,
                    Configuration = "1 vCPU, 1GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Starter+US"
                },
                new()
                {
                    Name = "Pro EU",
                    Location = "EU",
                    PricePerMonth = 15,
                    Configuration = "2 vCPU, 4GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Pro+EU"
                },
                new()
                {
                    Name = "Enterprise Asia",
                    Location = "Asia",
                    PricePerMonth = 30,
                    Configuration = "4 vCPU, 8GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Enterprise+Asia"
                }
            };

            context.Servers.AddRange(servers);
            context.SaveChanges();
        }

        if (!context.Orders.Any())
        {
            const string email = "test@example.com";
            const string password = "Pa$$w0rd";

            var user = context.Users.FirstOrDefault(u => u.UserName == email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var hasher = new PasswordHasher<IdentityUser>();
                user.PasswordHash = hasher.HashPassword(user, password);
                context.Users.Add(user);
                context.SaveChanges();
            }

            var orders = context.Servers
                .Select(s => new Order
                {
                    UserId = user.Id,
                    ServerId = s.Id,
                    TotalPrice = s.PricePerMonth,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Completed
                })
                .ToList();

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
