namespace CloudCityCenter.Models.Admin;

public sealed class PageVisitTimelineItemViewModel
{
    public DateTime VisitedAt { get; init; }
    public string HttpMethod { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string? QueryString { get; init; }
}
