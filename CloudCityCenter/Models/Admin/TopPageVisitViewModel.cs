namespace CloudCityCenter.Models.Admin;

public sealed class TopPageVisitViewModel
{
    public string Path { get; init; } = string.Empty;
    public int VisitsCount { get; init; }
    public DateTime LastVisitAt { get; init; }
}
