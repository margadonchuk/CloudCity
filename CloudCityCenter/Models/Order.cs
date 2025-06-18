using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int ServerId { get; set; }

    public Server? Server { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }
}
