namespace CloudCityCenter.Models.ViewModels;

public class ConfigurationServiceVm
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty; // CHR, MikroTik, Fortinet
}

public class ConfigurationServiceCategoryVm
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDescription { get; set; } = string.Empty;
    public List<ConfigurationServiceVm> Services { get; set; } = new List<ConfigurationServiceVm>();
}

public class VPNPageVm
{
    public List<ProductCardVm> VPNProducts { get; set; } = new List<ProductCardVm>();
    public List<ConfigurationServiceCategoryVm> ConfigurationServices { get; set; } = new List<ConfigurationServiceCategoryVm>();
}

