using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using CloudCityCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Data;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        MigrateLegacyServers(context);
        SeedProducts(context);

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

    private static void SeedProducts(ApplicationDbContext context)
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
                IsPublished = true,
                ImageUrl = "https://via.placeholder.com/300x200?text=Starter+US",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 5,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "1 vCPU" },
                    new() { Name = "RAM", Value = "1 GB" },
                    new() { Name = "Storage", Value = "25 GB SSD" }
                }
            },
            new()
            {
                Name = "Basic Hosting",
                Slug = "basic-hosting",
                Location = "US",
                PricePerMonth = 10,
                Configuration = "Shared hosting",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "https://via.placeholder.com/300x200?text=Hosting",
                Type = ProductType.Hosting,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 10,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "Storage", Value = "10 GB" },
                    new() { Name = "Bandwidth", Value = "100 GB" },
                    new() { Name = "Email Accounts", Value = "5" }
                }
            },
            new()
            {
                Name = "Starter Website",
                Slug = "starter-website",
                Location = "US",
                PricePerMonth = 20,
                Configuration = "WordPress site",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "https://via.placeholder.com/300x200?text=Website",
                Type = ProductType.Website,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 20,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "Pages", Value = "5" },
                    new() { Name = "Support", Value = "Email Support" },
                    new() { Name = "SSL", Value = "Included" }
                }
            }
        };

        foreach (var product in products)
        {
            if (!context.Products.Any(p => p.Slug == product.Slug))
            {
                context.Products.Add(product);
            }
        }
        context.SaveChanges();
    }

    private static void MigrateLegacyServers(ApplicationDbContext context)
    {
        try
        {
            using var connection = context.Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Name, Location, PricePerMonth, Configuration, IsAvailable, ImageUrl FROM Servers";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var name = reader["Name"]?.ToString() ?? string.Empty;
                var product = new Product
                {
                    Name = name,
                    Slug = name.ToLower().Replace(' ', '-'),
                    Location = reader["Location"]?.ToString() ?? string.Empty,
                    PricePerMonth = reader.GetFieldValue<decimal>(reader.GetOrdinal("PricePerMonth")),
                    Configuration = reader["Configuration"]?.ToString() ?? string.Empty,
                    IsAvailable = reader.GetFieldValue<bool>(reader.GetOrdinal("IsAvailable")),
                    ImageUrl = reader["ImageUrl"] as string,
                    Type = ProductType.DedicatedServer
                };

                if (!context.Products.Any(p => p.Name == product.Name))
                {
                    context.Products.Add(product);
                }
            }

            context.SaveChanges();
        }
        catch (Exception)
        {
            // Legacy Servers table not found; nothing to migrate
        }
    }
}
