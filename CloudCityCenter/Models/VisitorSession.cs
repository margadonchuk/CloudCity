using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class VisitorSession
{
    public int Id { get; set; }

    [Required]
    [StringLength(45)]
    public string IpAddress { get; set; } = string.Empty;

    [Required]
    public DateTime FirstSeenAt { get; set; }

    [Required]
    public DateTime LastSeenAt { get; set; }

    [StringLength(1024)]
    public string? UserAgent { get; set; }

    [StringLength(2048)]
    public string? Referrer { get; set; }

    [StringLength(128)]
    public string? Country { get; set; }

    [StringLength(128)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? CountryCode { get; set; }

    public ICollection<PageVisit> PageVisits { get; set; } = new List<PageVisit>();
}
