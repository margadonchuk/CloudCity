using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class Server
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal PricePerMonth { get; set; }

    [Required]
    [StringLength(200)]
    public string Configuration { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    [StringLength(300)]
    public string? ImageUrl { get; set; }
}
