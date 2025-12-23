namespace CloudCityCenter.Models.ViewModels;

public class VDIPlanVm
{
    public string Name { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int RamGb { get; set; }
    public int SsdGb { get; set; }
    public string Traffic { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

public class VDIPlansByRegionVm
{
    public string RegionName { get; set; } = string.Empty;
    public List<VDIPlanVm> Plans { get; set; } = new List<VDIPlanVm>();
}

public class VDIPageVm
{
    public List<VDIPlansByRegionVm> RegionsForOnePerson { get; set; } = new List<VDIPlansByRegionVm>();
    public List<VDIPlansByRegionVm> RegionsForThreePersons { get; set; } = new List<VDIPlansByRegionVm>();
}

