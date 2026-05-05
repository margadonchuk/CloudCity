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
    public IReadOnlyList<AnalyticsBreakdownItemViewModel> TopBrowsers { get; init; } = Array.Empty<AnalyticsBreakdownItemViewModel>();
    public IReadOnlyList<AnalyticsBreakdownItemViewModel> TopCountries { get; init; } = Array.Empty<AnalyticsBreakdownItemViewModel>();
    public IReadOnlyList<AnalyticsBreakdownItemViewModel> DeviceSplit { get; init; } = Array.Empty<AnalyticsBreakdownItemViewModel>();
    public IReadOnlyList<SuspiciousActivityListItemViewModel> SuspiciousActivities { get; init; } = Array.Empty<SuspiciousActivityListItemViewModel>();
    public IReadOnlyList<FunnelStepViewModel> FunnelSteps { get; init; } = Array.Empty<FunnelStepViewModel>();
    public string TopExitStep { get; init; } = "—";
}

public sealed class FunnelStepViewModel
{
    public string Name { get; init; } = string.Empty;
    public int Visitors { get; init; }
    public decimal DropOffPercentage { get; init; }
}

public sealed class AnalyticsBreakdownItemViewModel
{
    public string Label { get; init; } = string.Empty;
    public int Count { get; init; }
}

public sealed class SuspiciousActivityListItemViewModel
{
    public int VisitorSessionId { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public DateTime DetectedAtUtc { get; init; }
    public bool IsIpBlocked { get; init; }
}
