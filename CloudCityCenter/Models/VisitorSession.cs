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

    public ICollection<PageVisit> PageVisits { get; set; } = new List<PageVisit>();
}
