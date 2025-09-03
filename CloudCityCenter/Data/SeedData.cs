using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CloudCityCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Data;

public static class SeedData
{
    public static async Task RunAsync(IServiceProvider serviceProvider, string? adminEmail = null)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Admin", "Manager", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!string.IsNullOrWhiteSpace(adminEmail))
        {
            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

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
                Name = "VPS1-1-20",
                Slug = "vps1-1-20",
                Location = "Germany",
                PricePerMonth = 10,
                Configuration = "Intel 1 Core, 1GB RAM, 20GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps1.png",
                Type = ProductType.VPS,
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
                    new() { Name = "CPU", Value = "Intel 1 Core" },
                    new() { Name = "RAM", Value = "1GB" },
                    new() { Name = "Storage", Value = "20GB SSD" },
                    new() { Name = "Network", Value = "100Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France" }
                }
            },
            new()
            {
                Name = "VPS2-2-20",
                Slug = "vps2-2-20",
                Location = "Germany",
                PricePerMonth = 12,
                Configuration = "Intel 2 Cores, 2GB RAM, 20GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps2.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 12,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Intel 2 Cores" },
                    new() { Name = "RAM", Value = "2GB" },
                    new() { Name = "Storage", Value = "20GB SSD" },
                    new() { Name = "Network", Value = "250Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France" }
                }
            },
            new()
            {
                Name = "VPS3-4-40",
                Slug = "vps3-4-40",
                Location = "Germany",
                PricePerMonth = 19,
                Configuration = "AMD | Intel 4 Cores, 4GB RAM, 40GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps3.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 19,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "AMD | Intel 4 Cores" },
                    new() { Name = "RAM", Value = "4GB" },
                    new() { Name = "Storage", Value = "40GB SSD" },
                    new() { Name = "Network", Value = "250Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France | Netherlands" }
                }
            },
            new()
            {
                Name = "VPS4-4-80",
                Slug = "vps4-4-80",
                Location = "Germany",
                PricePerMonth = 24,
                Configuration = "AMD | Intel 4 Cores, 4GB RAM, 80GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps4.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 24,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "AMD | Intel 4 Cores" },
                    new() { Name = "RAM", Value = "4GB" },
                    new() { Name = "Storage", Value = "80GB SSD" },
                    new() { Name = "Network", Value = "500Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France | Netherlands | Singapore | USA" }
                }
            },
            new()
            {
                Name = "VPS5-4-80",
                Slug = "vps5-4-80",
                Location = "Germany",
                PricePerMonth = 28,
                Configuration = "AMD | Intel 4 Cores, 4GB RAM, 80GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps5.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 28,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "AMD | Intel 4 Cores" },
                    new() { Name = "RAM", Value = "4GB" },
                    new() { Name = "Storage", Value = "80GB SSD" },
                    new() { Name = "Network", Value = "1000Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France | Netherlands | Singapore | USA" }
                }
            },
            new()
            {
                Name = "VPS6-6-100",
                Slug = "vps6-6-100",
                Location = "Germany",
                PricePerMonth = 35,
                Configuration = "AMD | Intel 4 Cores, 6GB RAM, 100GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps6.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 35,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "AMD | Intel 4 Cores" },
                    new() { Name = "RAM", Value = "6GB" },
                    new() { Name = "Storage", Value = "100GB SSD" },
                    new() { Name = "Network", Value = "1000Mbps Bandwidth" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany | Poland | France | Netherlands | Singapore | USA" }
                }
            },
            new()
            {
                Name = "VPN for a network",
                Slug = "vpn-network",
                Location = "Global",
                PricePerMonth = 80,
                Configuration = "Site-to-site VPN service",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vpn-network.png",
                Type = ProductType.VPN,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 80,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "Devices", Value = "Up to 50" },
                    new() { Name = "Traffic", Value = "Unlimited" },
                    new() { Name = "Support", Value = "24/7" }
                }
            },
            new()
            {
                Name = "VPN for a device",
                Slug = "vpn-device",
                Location = "Global",
                PricePerMonth = 25,
                Configuration = "Single device VPN",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vpn-device.png",
                Type = ProductType.VPN,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 25,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "Traffic", Value = "Unlimited" },
                    new() { Name = "Servers", Value = "50+ Countries" },
                    new() { Name = "Encryption", Value = "AES-256" }
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
