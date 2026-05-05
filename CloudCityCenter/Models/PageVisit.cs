using System.ComponentModel.DataAnnotations;

namespace CloudCityCenter.Models;

public class PageVisit
{
    public int Id { get; set; }

    [Required]
    public int VisitorSessionId { get; set; }

    [Required]
    [StringLength(2048)]
    public string Path { get; set; } = string.Empty;

    [StringLength(2048)]
    public string? QueryString { get; set; }

    [Required]
    [StringLength(16)]
    public string HttpMethod { get; set; } = string.Empty;

    [Required]
    public DateTime VisitedAt { get; set; }

    public VisitorSession VisitorSession { get; set; } = null!;
}
