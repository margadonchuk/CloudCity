namespace CloudCityCenter.Models.Admin;

public sealed class VisitorSessionListItemViewModel
{
    public int Id { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public string NormalizedIpAddress { get; init; } = string.Empty;
    public DateTime FirstSeenAt { get; init; }
    public DateTime LastSeenAt { get; init; }
    public int PagesCount { get; init; }
    public string? UserAgent { get; init; }
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? CountryCode { get; init; }
    public string Browser { get; init; } = "Other";
    public string Device { get; init; } = "Desktop";
    public bool IsSuspicious { get; init; }
    public bool IsIpBlocked { get; init; }
}
