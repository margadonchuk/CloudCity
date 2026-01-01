namespace CloudCityCenter.Models.ViewModels;

public class VPSPlanVm
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int RamGb { get; set; }
    public int SsdGb { get; set; }
    public string Traffic { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

public class VPSPageVm
{
    public List<VPSPlanVm> Plans { get; set; } = new List<VPSPlanVm>();
    
    // Для фильтрации - все уникальные значения
    public List<int> AvailableCpuCores { get; set; } = new List<int>();
    public List<int> AvailableRamGb { get; set; } = new List<int>();
    public List<int> AvailableSsdGb { get; set; } = new List<int>();
    public List<string> AvailableTraffic { get; set; } = new List<string>();
}

