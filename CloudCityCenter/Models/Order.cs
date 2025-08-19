using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CloudCityCenter.Models;

public enum OrderStatus
{
    Pending,
    Completed,
    Canceled
}

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = string.Empty;

    public IdentityUser? User { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    [Range(0, double.MaxValue)]
    public decimal Total { get; set; }

    [StringLength(3)]
    public string Currency { get; set; } = "USD";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
