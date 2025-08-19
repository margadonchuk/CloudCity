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

        if (!context.Products.Any())
        {
            var products = new List<Product>
            {
                new()
                {
                    Name = "Starter US",
                    Slug = "starter-us",
                    Location = "US",
                    PricePerMonth = 5,
                    Configuration = "1 vCPU, 1GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Starter+US",
                    Type = ProductType.DedicatedServer
                },
                new()
                {
                    Name = "Pro EU",
                    Slug = "pro-eu",
                    Location = "EU",
                    PricePerMonth = 15,
                    Configuration = "2 vCPU, 4GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Pro+EU",
                    Type = ProductType.DedicatedServer
                },
                new()
                {
                    Name = "Enterprise Asia",
                    Slug = "enterprise-asia",
                    Location = "Asia",
                    PricePerMonth = 30,
                    Configuration = "4 vCPU, 8GB RAM",
                    IsAvailable = true,
                    ImageUrl = "https://via.placeholder.com/300x200?text=Enterprise+Asia",
                    Type = ProductType.DedicatedServer
                }
            };

            context.Products.AddRange(products);
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

            var orders = context.Products
                .Where(p => p.Type == ProductType.DedicatedServer)
                .Select(p => new Order
                {
                    UserId = user.Id,
                    Items = new List<OrderItem> { new OrderItem { ProductId = p.Id, Price = p.PricePerMonth } },
                    Total = p.PricePerMonth,
                    Currency = "USD",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Completed
                })
                .ToList();

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
