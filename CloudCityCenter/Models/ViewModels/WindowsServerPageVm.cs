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

public class WindowsServerPageVm
{
    public List<WindowsServerPlanVm> PlansForFiveToEightPersons { get; set; } = new List<WindowsServerPlanVm>();
    public List<WindowsServerPlanVm> PlansForFifteenPersons { get; set; } = new List<WindowsServerPlanVm>();
    public List<WindowsServerPlanVm> PlansForTwentyFivePersons { get; set; } = new List<WindowsServerPlanVm>();
    public List<WindowsServerPlanVm> PlansForThirtyFivePersons { get; set; } = new List<WindowsServerPlanVm>();
    public List<WindowsServerPlanVm> PlansForFiftyPersons { get; set; } = new List<WindowsServerPlanVm>();
}

