namespace CloudCityCenter.Models.Admin;

public sealed class VisitorSessionListItemViewModel
{
    public int Id { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public DateTime FirstSeenAt { get; init; }
    public DateTime LastSeenAt { get; init; }
    public int PagesCount { get; init; }
    public string? UserAgent { get; init; }
}
