using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;

namespace CloudCityCenter.Controllers;

public class VPNController : Controller
{
    private readonly ApplicationDbContext _context;

    public VPNController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vpnProducts = await _context.Products
            .Where(p => p.Type == ProductType.VPN && p.IsPublished)
            .Include(p => p.Features)
            .ToListAsync();

        var productCards = vpnProducts.Select(p =>
        {
            var featuresDict = p.Features.ToDictionary(f => f.Name, f => f.Value);

            // Порядок отображения фичей для VPN продуктов
            string[] featureOrder = new[] { "Technology", "Devices", "Traffic", "Encryption", "Country" };
            int maxFeatures = 10;

            var orderedFeatures = new List<string>();

            foreach (var featureName in featureOrder)
            {
                if (featuresDict.TryGetValue(featureName, out var value))
                {
                    orderedFeatures.Add(string.IsNullOrWhiteSpace(value)
                        ? featureName
                        : $"{featureName}: {value}");
                }
            }

            // Если есть другие фичи, которые не в списке, добавляем их в конце
            foreach (var feature in p.Features.OrderBy(f => f.Id))
            {
                if (!featureOrder.Contains(feature.Name) && !orderedFeatures.Any(f => f.StartsWith(feature.Name)))
                {
                    orderedFeatures.Add(string.IsNullOrWhiteSpace(feature.Value)
                        ? feature.Name
                        : $"{feature.Name}: {feature.Value}");
                }
            }

            return new ProductCardVm
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                PricePerMonth = p.PricePerMonth,
                ImageUrl = p.ImageUrl,
                TopFeatures = orderedFeatures.Take(maxFeatures).ToList()
            };
        }).ToList();

        // Услуги настройки оборудования
        var configurationServices = GetConfigurationServices();

        var viewModel = new VPNPageVm
        {
            VPNProducts = productCards,
            ConfigurationServices = configurationServices
        };

        return View(viewModel);
    }

    private List<ConfigurationServiceCategoryVm> GetConfigurationServices()
    {
        return new List<ConfigurationServiceCategoryVm>
        {
            new ConfigurationServiceCategoryVm
            {
                CategoryName = "Настройка CHR (Cloud Hosted Router)",
                CategoryDescription = "Установка/обновление RouterOS на виртуальном сервере, настройка VPN (WireGuard, IPsec, OpenVPN), Firewall, NAT, проброс портов, маршрутизация, мониторинг, базовые политики доступа, оптимизация под сеть клиента",
                Services = new List<ConfigurationServiceVm>
                {
                    new ConfigurationServiceVm
                    {
                        Name = "CHR Basic",
                        Description = "Базовая настройка CHR (VPN, Firewall, NAT)",
                        Price = 150,
                        Category = "CHR",
                        ImageUrl = "/images/chr1.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "CHR Standard",
                        Description = "Расширенные правила безопасности + мониторинг",
                        Price = 250,
                        Category = "CHR",
                        ImageUrl = "/images/chr2.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "CHR Pro",
                        Description = "Полная настройка с резервами, отказоустойчивостью",
                        Price = 399,
                        Category = "CHR",
                        ImageUrl = "/images/chr3.png"
                    }
                }
            },
            new ConfigurationServiceCategoryVm
            {
                CategoryName = "Настройка физического оборудования MikroTik",
                CategoryDescription = "",
                Services = new List<ConfigurationServiceVm>
                {
                    new ConfigurationServiceVm
                    {
                        Name = "MikroTik Basic",
                        Description = "WAN/LAN подключение, NAT, DHCP, базовый Firewall",
                        Price = 180,
                        Category = "MikroTik",
                        ImageUrl = "/images/mikrotik1.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "MikroTik Advanced",
                        Description = "VPN/IPsec/L2TP, VLAN, резервирование провайдеров",
                        Price = 250,
                        Category = "MikroTik",
                        ImageUrl = "/images/mikrotik2.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "MikroTik Pro",
                        Description = "WiFi CAPsMAN, комплексное сетевое решение, мониторинг",
                        Price = 450,
                        Category = "MikroTik",
                        ImageUrl = "/images/mikrotik3.png"
                    }
                }
            },
            new ConfigurationServiceCategoryVm
            {
                CategoryName = "Настройка физического оборудования Fortinet",
                CategoryDescription = "Fortinet — это техника уровня корпоративной сетевой безопасности, сложнее MikroTik, и её настройка требует большего времени и опыта. Fortinet требует продвинутой сертификации и времени (часто 4–8 часов и больше), поэтому тарифы выше среднего уровня.",
                Services = new List<ConfigurationServiceVm>
                {
                    new ConfigurationServiceVm
                    {
                        Name = "Fortinet Basic",
                        Description = "Настройка базовых правил, интерфейсов, Firewall",
                        Price = 400,
                        Category = "Fortinet",
                        ImageUrl = "/images/fortinet1.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "Fortinet Security",
                        Description = "VPN (IPsec/SSL), высокоуровневое шифрование, IPS",
                        Price = 770,
                        Category = "Fortinet",
                        ImageUrl = "/images/fortinet2.png"
                    },
                    new ConfigurationServiceVm
                    {
                        Name = "Fortinet Enterprise",
                        Description = "Полный сетевой UTM, сегментация, HA, логирование",
                        Price = 1500,
                        Category = "Fortinet",
                        ImageUrl = "/images/fortinet3.png"
                    }
                }
            }
        };
    }
}
