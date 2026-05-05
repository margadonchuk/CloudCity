namespace CloudCityCenter.Models.Admin;

public sealed class AnalyticsIndexViewModel
{
    public IReadOnlyList<VisitorSessionListItemViewModel> Visitors { get; init; } = Array.Empty<VisitorSessionListItemViewModel>();
    public IReadOnlyList<TopPageVisitViewModel> TopPages { get; init; } = Array.Empty<TopPageVisitViewModel>();
}
