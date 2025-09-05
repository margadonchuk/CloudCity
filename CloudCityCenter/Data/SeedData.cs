using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
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
            var password = Environment.GetEnvironmentVariable("SEED_USER_PASSWORD");
            if (string.IsNullOrEmpty(password))
            {
                password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
                Console.WriteLine($"Generated password for seed user {email}: {password}");
            }

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
                Name = "DS1-E3",
                Slug = "ds1-e3",
                Location = "Germany",
                PricePerMonth = 59,
                Configuration = "Xeon E3-1270v6, 32GB RAM, 2x240GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/dell.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 59,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Xeon E3-1270v6" },
                    new() { Name = "RAM", Value = "32GB" },
                    new() { Name = "Storage", Value = "2x240GB SSD" },
                    new() { Name = "Network", Value = "1Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Germany" }
                }
            },
            new()
            {
                Name = "DS2-E5",
                Slug = "ds2-e5",
                Location = "Netherlands",
                PricePerMonth = 79,
                Configuration = "Xeon E5-1650v4, 64GB RAM, 2x480GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/hp.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 79,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Xeon E5-1650v4" },
                    new() { Name = "RAM", Value = "64GB" },
                    new() { Name = "Storage", Value = "2x480GB SSD" },
                    new() { Name = "Network", Value = "1Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "DS3-Ryzen",
                Slug = "ds3-ryzen",
                Location = "France",
                PricePerMonth = 95,
                Configuration = "Ryzen 5 3600, 64GB RAM, 2x1TB NVMe",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/ovh.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 95,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Ryzen 5 3600" },
                    new() { Name = "RAM", Value = "64GB" },
                    new() { Name = "Storage", Value = "2x1TB NVMe" },
                    new() { Name = "Network", Value = "1Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "France" }
                }
            },
            new()
            {
                Name = "DS4-EPYC",
                Slug = "ds4-epyc",
                Location = "Finland",
                PricePerMonth = 129,
                Configuration = "AMD EPYC 7501, 128GB RAM, 2x1.92TB NVMe",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/hetzner.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 129,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "AMD EPYC 7501" },
                    new() { Name = "RAM", Value = "128GB" },
                    new() { Name = "Storage", Value = "2x1.92TB NVMe" },
                    new() { Name = "Network", Value = "2Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Finland" }
                }
            },
            new()
            {
                Name = "DS5-Storage",
                Slug = "ds5-storage",
                Location = "USA",
                PricePerMonth = 149,
                Configuration = "Xeon Silver 4210, 64GB RAM, 4x4TB HDD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/proxmox.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 149,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Xeon Silver 4210" },
                    new() { Name = "RAM", Value = "64GB" },
                    new() { Name = "Storage", Value = "4x4TB HDD" },
                    new() { Name = "Network", Value = "1Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "USA" }
                }
            },
            new()
            {
                Name = "DS6-HighMem",
                Slug = "ds6-highmem",
                Location = "Singapore",
                PricePerMonth = 169,
                Configuration = "Xeon Gold 6130, 256GB RAM, 2x960GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/ms.png",
                Type = ProductType.DedicatedServer,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 169,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "Xeon Gold 6130" },
                    new() { Name = "RAM", Value = "256GB" },
                    new() { Name = "Storage", Value = "2x960GB SSD" },
                    new() { Name = "Network", Value = "2Gbps" },
                    new() { Name = "IPv4", Value = "1" },
                    new() { Name = "Location", Value = "Singapore" }
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
