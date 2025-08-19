namespace CloudCityCenter.Models.ViewModels;

public class ProductCardVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal PricePerMonth { get; set; }
    public string Configuration { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
