using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class BlockedIp
{
    public int Id { get; set; }

    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(45)]
    public string NormalizedIpAddress { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Reason { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    public bool IsActive { get; set; } = true;
}
