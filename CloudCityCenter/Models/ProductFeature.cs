using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCityCenter.Models;

public class ProductFeature
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Value { get; set; }

    public Product? Product { get; set; }
}
