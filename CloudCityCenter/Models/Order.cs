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

    [Required]
    public int ServerId { get; set; }

    public Server? Server { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal TotalPrice { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
}
