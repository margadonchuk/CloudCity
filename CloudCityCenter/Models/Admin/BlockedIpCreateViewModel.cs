using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models.Admin;

public class BlockedIpCreateViewModel
{
    [Required(ErrorMessage = "IP address is required.")]
    [Display(Name = "IP address")]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [Display(Name = "Reason")]
    [StringLength(500)]
    public string? Reason { get; set; }
}
