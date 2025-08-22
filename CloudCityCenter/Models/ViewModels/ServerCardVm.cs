namespace CloudCityCenter.Models.ViewModels;

/// <summary>
/// Lightweight view model used to render a server card on list pages.
/// </summary>
public record ServerCardVm
{
    public string? ImageUrl { get; init; }
    public string Name { get; init; } = string.Empty;
    public string CPU { get; init; } = string.Empty;
    public int RamGb { get; init; }
    public int StorageGb { get; init; }
    public string? Location { get; init; }
    public decimal PricePerMonth { get; init; }
    public string Slug { get; init; } = string.Empty;
}

