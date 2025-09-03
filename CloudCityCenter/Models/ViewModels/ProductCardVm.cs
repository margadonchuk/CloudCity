namespace CloudCityCenter.Models.ViewModels;

/// <summary>
/// View model used to render a product card.
/// </summary>
public record ProductCardVm
{
    public int Id { get; init; }
    public int? ProductVariantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public decimal PricePerMonth { get; init; }
    public string? ImageUrl { get; init; }

    /// <summary>
    /// A collection of feature descriptions to display for the product.
    /// </summary>
    public IEnumerable<string> TopFeatures { get; init; } = new List<string>();
}
