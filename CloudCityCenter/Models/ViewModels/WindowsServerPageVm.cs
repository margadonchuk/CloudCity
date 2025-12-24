namespace CloudCityCenter.Models.ViewModels;

public class WindowsServerPlanVm
{
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CPU { get; set; } = string.Empty;
    public string RAM { get; set; } = string.Empty;
    public string SSD { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int NumberOfPersons { get; set; } // Для фильтрации по количеству человек
}

public class WindowsServerPlansByRegionVm
{
    public string RegionName { get; set; } = string.Empty;
    public List<WindowsServerPlanVm> Plans { get; set; } = new List<WindowsServerPlanVm>();
}

public class WindowsServerPageVm
{
    public List<WindowsServerPlansByRegionVm> RegionsForFiveToEightPersons { get; set; } = new List<WindowsServerPlansByRegionVm>();
    public List<WindowsServerPlansByRegionVm> RegionsForFifteenPersons { get; set; } = new List<WindowsServerPlansByRegionVm>();
    public List<WindowsServerPlansByRegionVm> RegionsForTwentyFivePersons { get; set; } = new List<WindowsServerPlansByRegionVm>();
    public List<WindowsServerPlansByRegionVm> RegionsForThirtyFivePersons { get; set; } = new List<WindowsServerPlansByRegionVm>();
    public List<WindowsServerPlansByRegionVm> RegionsForFiftyPersons { get; set; } = new List<WindowsServerPlansByRegionVm>();
}

