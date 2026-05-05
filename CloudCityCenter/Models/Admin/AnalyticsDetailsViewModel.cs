namespace CloudCityCenter.Models.Admin;

public sealed class AnalyticsDetailsViewModel
{
    public int VisitorSessionId { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public DateTime FirstSeenAt { get; init; }
    public DateTime LastSeenAt { get; init; }
    public string? UserAgent { get; init; }
    public string? Referrer { get; init; }
    public IReadOnlyList<PageVisitTimelineItemViewModel> Visits { get; init; } = Array.Empty<PageVisitTimelineItemViewModel>();
}
