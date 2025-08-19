using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ProductType Type { get; set; }

    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal PricePerMonth { get; set; }

    [StringLength(200)]
    public string Configuration { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    [StringLength(300)]
    public string? ImageUrl { get; set; }

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    public ICollection<ProductFeature> Features { get; set; } = new List<ProductFeature>();
}
