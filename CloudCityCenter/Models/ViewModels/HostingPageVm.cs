using System.Collections.Generic;

namespace CloudCityCenter.Models.ViewModels;

public class HostingPageVm
{
    public IEnumerable<ProductCardVm> HostingPlans { get; set; } = new List<ProductCardVm>();
    public IEnumerable<ProductCardVm> WebsiteProducts { get; set; } = new List<ProductCardVm>();
    public IEnumerable<ProductCardVm> VpsProducts { get; set; } = new List<ProductCardVm>();
    public IEnumerable<ProductCardVm> VpnProducts { get; set; } = new List<ProductCardVm>();
    public IEnumerable<ProductCardVm> StorageProducts { get; set; } = new List<ProductCardVm>();
}
