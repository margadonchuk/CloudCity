namespace CloudCityCenter.Models.Admin;

public sealed class VisitorSessionListItemViewModel
{
    public string IpAddress { get; init; } = string.Empty;
    public DateTime FirstSeenAt { get; init; }
    public DateTime LastSeenAt { get; init; }
}
