using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCityCenter.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int? ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
