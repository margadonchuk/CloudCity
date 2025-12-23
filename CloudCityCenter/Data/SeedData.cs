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
            // VPS Products
            new()
            {
                Name = "VPS1-1-20",
                Slug = "vps1-1-20",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 10,
                Configuration = "1 Core, 1GB RAM, 20GB SSD",
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
                    new() { Name = "CPU", Value = "1 core" },
                    new() { Name = "RAM", Value = "1 GB" },
                    new() { Name = "SSD", Value = "20 GB" },
                    new() { Name = "Traffic", Value = "100 mb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS2-2-40",
                Slug = "vps2-2-40",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 20,
                Configuration = "2 Cores, 2GB RAM, 40GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps2.png",
                Type = ProductType.VPS,
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
                    new() { Name = "CPU", Value = "2 core" },
                    new() { Name = "RAM", Value = "2 GB" },
                    new() { Name = "SSD", Value = "40 GB" },
                    new() { Name = "Traffic", Value = "250 mb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS3-2-150",
                Slug = "vps3-2-150",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 35,
                Configuration = "2 Cores, 4GB RAM, 150GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps3.png",
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
                    new() { Name = "CPU", Value = "2 core" },
                    new() { Name = "RAM", Value = "4 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "250 mb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS4-4-80",
                Slug = "vps4-4-80",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 49,
                Configuration = "4 Cores, 4GB RAM, 80GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps4.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 49,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "4 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "500 mb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS5-6-100",
                Slug = "vps5-6-100",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 80,
                Configuration = "6 Cores, 12GB RAM, 100GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps5.png",
                Type = ProductType.VPS,
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
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "100 GB" },
                    new() { Name = "Traffic", Value = "1 Gb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS6-8-120",
                Slug = "vps6-8-120",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 95,
                Configuration = "8 Cores, 8GB RAM, 120GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps6.png",
                Type = ProductType.VPS,
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
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "500 mb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS7-8-160",
                Slug = "vps7-8-160",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 150,
                Configuration = "8 Cores, 16GB RAM, 160GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps7.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 150,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "160 GB" },
                    new() { Name = "Traffic", Value = "1 Gb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS8-8-200",
                Slug = "vps8-8-200",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 175,
                Configuration = "8 Cores, 24GB RAM, 200GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps8.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 175,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "24 GB" },
                    new() { Name = "SSD", Value = "200 GB" },
                    new() { Name = "Traffic", Value = "1 Gb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS9-12-300",
                Slug = "vps9-12-300",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 220,
                Configuration = "12 Cores, 48GB RAM, 300GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps9.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 220,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "12 core" },
                    new() { Name = "RAM", Value = "48 GB" },
                    new() { Name = "SSD", Value = "300 GB" },
                    new() { Name = "Traffic", Value = "1 Gb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
                }
            },
            new()
            {
                Name = "VPS10-16-320",
                Slug = "vps10-16-320",
                Location = "Netherlands/Germany/France",
                PricePerMonth = 249,
                Configuration = "16 Cores, 32GB RAM, 320GB SSD",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vps10.png",
                Type = ProductType.VPS,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 249,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "16 core" },
                    new() { Name = "RAM", Value = "32 GB" },
                    new() { Name = "SSD", Value = "320 GB" },
                    new() { Name = "Traffic", Value = "1 Gb" },
                    new() { Name = "Country", Value = "Netherlands/Germany/France" },
                    new() { Name = "OS", Value = "Linux" }
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
            },
            // VDI Products - Netherlands
            new()
            {
                Name = "VDI Start - Netherlands",
                Slug = "vdi-start-netherlands",
                Location = "Netherlands",
                PricePerMonth = 150,
                Configuration = "VDI на 1 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 150,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Standard - Netherlands",
                Slug = "vdi-standard-netherlands",
                Location = "Netherlands",
                PricePerMonth = 175,
                Configuration = "VDI на 1 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 175,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Pro - Netherlands",
                Slug = "vdi-pro-netherlands",
                Location = "Netherlands",
                PricePerMonth = 200,
                Configuration = "VDI на 1 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 200,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            // VDI Products - Germany/France/Poland
            new()
            {
                Name = "VDI Start - Germany/France/Poland",
                Slug = "vdi-start-europe",
                Location = "Germany/France/Poland",
                PricePerMonth = 155,
                Configuration = "VDI на 1 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 155,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Standard - Germany/France/Poland",
                Slug = "vdi-standard-europe",
                Location = "Germany/France/Poland",
                PricePerMonth = 180,
                Configuration = "VDI на 1 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 180,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Pro - Germany/France/Poland",
                Slug = "vdi-pro-europe",
                Location = "Germany/France/Poland",
                PricePerMonth = 205,
                Configuration = "VDI на 1 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 205,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            // VDI Products - USA/Canada/Asia
            new()
            {
                Name = "VDI Start - USA/Canada/Asia",
                Slug = "vdi-start-usa-asia",
                Location = "USA/Canada/Asia",
                PricePerMonth = 175,
                Configuration = "VDI на 1 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 175,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Standard - USA/Canada/Asia",
                Slug = "vdi-standard-usa-asia",
                Location = "USA/Canada/Asia",
                PricePerMonth = 200,
                Configuration = "VDI на 1 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 200,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Pro - USA/Canada/Asia",
                Slug = "vdi-pro-usa-asia",
                Location = "USA/Canada/Asia",
                PricePerMonth = 225,
                Configuration = "VDI на 1 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 225,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            // VDI Products for 3 persons - Netherlands
            new()
            {
                Name = "VDI Start - Netherlands (3 persons)",
                Slug = "vdi-start-netherlands-3",
                Location = "Netherlands",
                PricePerMonth = 390,
                Configuration = "VDI на 3 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start3_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 390,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Standard - Netherlands (3 persons)",
                Slug = "vdi-standard-netherlands-3",
                Location = "Netherlands",
                PricePerMonth = 465,
                Configuration = "VDI на 3 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt3_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 465,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Pro - Netherlands (3 persons)",
                Slug = "vdi-pro-netherlands-3",
                Location = "Netherlands",
                PricePerMonth = 540,
                Configuration = "VDI на 3 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro3_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 540,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            // VDI Products for 3 persons - Germany/France/Poland
            new()
            {
                Name = "VDI Start - Germany/France/Poland (3 persons)",
                Slug = "vdi-start-europe-3",
                Location = "Germany/France/Poland",
                PricePerMonth = 405,
                Configuration = "VDI на 3 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start3_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 405,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Standard - Germany/France/Poland (3 persons)",
                Slug = "vdi-standard-europe-3",
                Location = "Germany/France/Poland",
                PricePerMonth = 480,
                Configuration = "VDI на 3 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt3_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 480,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Pro - Germany/France/Poland (3 persons)",
                Slug = "vdi-pro-europe-3",
                Location = "Germany/France/Poland",
                PricePerMonth = 555,
                Configuration = "VDI на 3 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro3_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 555,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            // VDI Products for 3 persons - USA/Canada/Asia
            new()
            {
                Name = "VDI Start - USA/Canada/Asia (3 persons)",
                Slug = "vdi-start-usa-asia-3",
                Location = "USA/Canada/Asia",
                PricePerMonth = 465,
                Configuration = "VDI на 3 человека - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start3_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 465,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Standard - USA/Canada/Asia (3 persons)",
                Slug = "vdi-standard-usa-asia-3",
                Location = "USA/Canada/Asia",
                PricePerMonth = 540,
                Configuration = "VDI на 3 человека - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt3_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 540,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Pro - USA/Canada/Asia (3 persons)",
                Slug = "vdi-pro-usa-asia-3",
                Location = "USA/Canada/Asia",
                PricePerMonth = 615,
                Configuration = "VDI на 3 человека - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro3_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 615,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            // VDI Products for 5 persons - Netherlands
            new()
            {
                Name = "VDI Start - Netherlands (5 persons)",
                Slug = "vdi-start-netherlands-5",
                Location = "Netherlands",
                PricePerMonth = 550,
                Configuration = "VDI на 5 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start5_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 550,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Standard - Netherlands (5 persons)",
                Slug = "vdi-standard-netherlands-5",
                Location = "Netherlands",
                PricePerMonth = 675,
                Configuration = "VDI на 5 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt5_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 675,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Pro - Netherlands (5 persons)",
                Slug = "vdi-pro-netherlands-5",
                Location = "Netherlands",
                PricePerMonth = 800,
                Configuration = "VDI на 5 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro5_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 800,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            // VDI Products for 5 persons - Germany/France/Poland
            new()
            {
                Name = "VDI Start - Germany/France/Poland (5 persons)",
                Slug = "vdi-start-europe-5",
                Location = "Germany/France/Poland",
                PricePerMonth = 575,
                Configuration = "VDI на 5 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start5_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 575,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Standard - Germany/France/Poland (5 persons)",
                Slug = "vdi-standard-europe-5",
                Location = "Germany/France/Poland",
                PricePerMonth = 700,
                Configuration = "VDI на 5 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt5_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 700,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Pro - Germany/France/Poland (5 persons)",
                Slug = "vdi-pro-europe-5",
                Location = "Germany/France/Poland",
                PricePerMonth = 825,
                Configuration = "VDI на 5 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro5_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 825,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            // VDI Products for 5 persons - USA/Canada/Asia
            new()
            {
                Name = "VDI Start - USA/Canada/Asia (5 persons)",
                Slug = "vdi-start-usa-asia-5",
                Location = "USA/Canada/Asia",
                PricePerMonth = 675,
                Configuration = "VDI на 5 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start5_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 675,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Standard - USA/Canada/Asia (5 persons)",
                Slug = "vdi-standard-usa-asia-5",
                Location = "USA/Canada/Asia",
                PricePerMonth = 800,
                Configuration = "VDI на 5 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt5_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 800,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Pro - USA/Canada/Asia (5 persons)",
                Slug = "vdi-pro-usa-asia-5",
                Location = "USA/Canada/Asia",
                PricePerMonth = 925,
                Configuration = "VDI на 5 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro5_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 925,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            // VDI Products for 10 persons - Netherlands
            new()
            {
                Name = "VDI Start - Netherlands (10 persons)",
                Slug = "vdi-start-netherlands-10",
                Location = "Netherlands",
                PricePerMonth = 900,
                Configuration = "VDI на 10 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start10_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 900,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Standard - Netherlands (10 persons)",
                Slug = "vdi-standard-netherlands-10",
                Location = "Netherlands",
                PricePerMonth = 1150,
                Configuration = "VDI на 10 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt10_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1150,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Pro - Netherlands (10 persons)",
                Slug = "vdi-pro-netherlands-10",
                Location = "Netherlands",
                PricePerMonth = 1400,
                Configuration = "VDI на 10 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro10_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1400,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            // VDI Products for 10 persons - Germany/France/Poland
            new()
            {
                Name = "VDI Start - Germany/France/Poland (10 persons)",
                Slug = "vdi-start-europe-10",
                Location = "Germany/France/Poland",
                PricePerMonth = 950,
                Configuration = "VDI на 10 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start10_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 950,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Standard - Germany/France/Poland (10 persons)",
                Slug = "vdi-standard-europe-10",
                Location = "Germany/France/Poland",
                PricePerMonth = 1200,
                Configuration = "VDI на 10 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt10_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1200,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Pro - Germany/France/Poland (10 persons)",
                Slug = "vdi-pro-europe-10",
                Location = "Germany/France/Poland",
                PricePerMonth = 1450,
                Configuration = "VDI на 10 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro10_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1450,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            // VDI Products for 10 persons - USA/Canada/Asia
            new()
            {
                Name = "VDI Start - USA/Canada/Asia (10 persons)",
                Slug = "vdi-start-usa-asia-10",
                Location = "USA/Canada/Asia",
                PricePerMonth = 1150,
                Configuration = "VDI на 10 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start10_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1150,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Standard - USA/Canada/Asia (10 persons)",
                Slug = "vdi-standard-usa-asia-10",
                Location = "USA/Canada/Asia",
                PricePerMonth = 1400,
                Configuration = "VDI на 10 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt10_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1400,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Pro - USA/Canada/Asia (10 persons)",
                Slug = "vdi-pro-usa-asia-10",
                Location = "USA/Canada/Asia",
                PricePerMonth = 1650,
                Configuration = "VDI на 10 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro10_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1650,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            // VDI Products for 20 persons - Netherlands
            new()
            {
                Name = "VDI Start - Netherlands (20 persons)",
                Slug = "vdi-start-netherlands-20",
                Location = "Netherlands",
                PricePerMonth = 1500,
                Configuration = "VDI на 20 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start20_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1500,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Standard - Netherlands (20 persons)",
                Slug = "vdi-standard-netherlands-20",
                Location = "Netherlands",
                PricePerMonth = 2000,
                Configuration = "VDI на 20 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt20_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2000,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            new()
            {
                Name = "VDI Pro - Netherlands (20 persons)",
                Slug = "vdi-pro-netherlands-20",
                Location = "Netherlands",
                PricePerMonth = 2500,
                Configuration = "VDI на 20 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro20_1.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2500,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Netherlands" }
                }
            },
            // VDI Products for 20 persons - Germany/France/Poland
            new()
            {
                Name = "VDI Start - Germany/France/Poland (20 persons)",
                Slug = "vdi-start-europe-20",
                Location = "Germany/France/Poland",
                PricePerMonth = 1600,
                Configuration = "VDI на 20 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start20_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 1600,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Standard - Germany/France/Poland (20 persons)",
                Slug = "vdi-standard-europe-20",
                Location = "Germany/France/Poland",
                PricePerMonth = 2100,
                Configuration = "VDI на 20 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt20_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2100,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            new()
            {
                Name = "VDI Pro - Germany/France/Poland (20 persons)",
                Slug = "vdi-pro-europe-20",
                Location = "Germany/France/Poland",
                PricePerMonth = 2600,
                Configuration = "VDI на 20 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro20_2.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2600,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "Germany/France/Poland" }
                }
            },
            // VDI Products for 20 persons - USA/Canada/Asia
            new()
            {
                Name = "VDI Start - USA/Canada/Asia (20 persons)",
                Slug = "vdi-start-usa-asia-20",
                Location = "USA/Canada/Asia",
                PricePerMonth = 2000,
                Configuration = "VDI на 20 человек - Start",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_start20_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2000,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "4 core" },
                    new() { Name = "RAM", Value = "8 GB" },
                    new() { Name = "SSD", Value = "80 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Standard - USA/Canada/Asia (20 persons)",
                Slug = "vdi-standard-usa-asia-20",
                Location = "USA/Canada/Asia",
                PricePerMonth = 2500,
                Configuration = "VDI на 20 человек - Standard",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_standrt20_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 2500,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "6 core" },
                    new() { Name = "RAM", Value = "12 GB" },
                    new() { Name = "SSD", Value = "120 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            },
            new()
            {
                Name = "VDI Pro - USA/Canada/Asia (20 persons)",
                Slug = "vdi-pro-usa-asia-20",
                Location = "USA/Canada/Asia",
                PricePerMonth = 3000,
                Configuration = "VDI на 20 человек - Pro",
                IsAvailable = true,
                IsPublished = true,
                ImageUrl = "/images/vdi_pro20_3.png",
                Type = ProductType.VDI,
                Variants = new List<ProductVariant>
                {
                    new()
                    {
                        Name = "Monthly",
                        Price = 3000,
                        BillingPeriod = BillingPeriod.Monthly
                    }
                },
                Features = new List<ProductFeature>
                {
                    new() { Name = "CPU", Value = "8 core" },
                    new() { Name = "RAM", Value = "16 GB" },
                    new() { Name = "SSD", Value = "150 GB" },
                    new() { Name = "Traffic", Value = "1 Gb/s" },
                    new() { Name = "Country", Value = "USA/Canada/Asia" }
                }
            }
        };

        var addedCount = 0;
        var skippedCount = 0;
        
        foreach (var product in products)
        {
            try
            {
                // Используем синхронную проверку, так как мы уже в синхронном методе
                // и контекст должен быть открыт
                var exists = context.Products.Any(p => p.Slug == product.Slug);
                if (!exists)
                {
                    context.Products.Add(product);
                    addedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            catch (Exception ex) when (ex.Message.Contains("Invalid column name") || ex.Message.Contains("slug"))
            {
                // Если колонка slug не существует, проверяем по имени
                Console.WriteLine($"⚠ Колонка 'slug' не найдена, проверяю по имени...");
                var exists = context.Products.Any(p => p.Name == product.Name);
                if (!exists)
                {
                    context.Products.Add(product);
                    addedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
        }
        
        if (addedCount > 0)
        {
            context.SaveChanges();
            Console.WriteLine($"  Добавлено товаров: {addedCount}, пропущено (уже существуют): {skippedCount}");
        }
        else
        {
            Console.WriteLine($"  Все товары уже существуют в базе (пропущено: {skippedCount})");
        }
    }

    private static void MigrateLegacyServers(ApplicationDbContext context)
    {
        try
        {
            var connection = context.Database.GetDbConnection();
            var shouldClose = connection.State != System.Data.ConnectionState.Open;
            if (shouldClose)
            {
                connection.Open();
            }

            try
            {
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
            finally
            {
                if (shouldClose)
                {
                    connection.Close();
                }
            }
        }
        catch (Exception)
        {
            // Legacy Servers table not found; nothing to migrate
        }
    }
}
