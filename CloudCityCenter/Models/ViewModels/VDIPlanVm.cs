namespace CloudCityCenter.Models.ViewModels;

public class VDIPlanVm
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CpuCores { get; set; }
    public int RamGb { get; set; }
    public int SsdGb { get; set; }
    public string Traffic { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int NumberOfPersons { get; set; } // Для фильтрации по количеству человек
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
    public List<VDIPlansByRegionVm> RegionsForFivePersons { get; set; } = new List<VDIPlansByRegionVm>();
    public List<VDIPlansByRegionVm> RegionsForTenPersons { get; set; } = new List<VDIPlansByRegionVm>();
    public List<VDIPlansByRegionVm> RegionsForTwentyPersons { get; set; } = new List<VDIPlansByRegionVm>();
    
    // Для фильтрации - все уникальные значения
    public List<string> AvailableLocations { get; set; } = new List<string>();
    public List<int> AvailablePersons { get; set; } = new List<int>();
    public List<int> AvailableCpuCores { get; set; } = new List<int>();
    public List<int> AvailableRamGb { get; set; } = new List<int>();
    public List<int> AvailableSsdGb { get; set; } = new List<int>();
}

