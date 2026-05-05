namespace CloudCityCenter.Models.Admin;

public sealed class AnalyticsIndexViewModel
{
    public string SelectedFilter { get; init; } = "today";
    public string SearchIpAddress { get; init; } = string.Empty;
    public DateTime? StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
    public int TotalPageVisits { get; init; }
    public int OnlineNowCount { get; init; }
    public IReadOnlyList<VisitorSessionListItemViewModel> ActiveVisitors { get; init; } = Array.Empty<VisitorSessionListItemViewModel>();
    public IReadOnlyList<VisitorSessionListItemViewModel> Visitors { get; init; } = Array.Empty<VisitorSessionListItemViewModel>();
    public IReadOnlyList<TopPageVisitViewModel> TopPages { get; init; } = Array.Empty<TopPageVisitViewModel>();
}
