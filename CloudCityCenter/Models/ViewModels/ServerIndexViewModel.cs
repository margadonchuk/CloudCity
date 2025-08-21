namespace CloudCityCenter.Models.ViewModels;

public class ServerIndexViewModel
{
    public IEnumerable<ServerCardVm> Servers { get; set; } = new List<ServerCardVm>();

    public string? Location { get; set; }
    public int? MinRam { get; set; }
    public int? MaxRam { get; set; }
    public string? Q { get; set; }
    public string? Sort { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public string FiltersSummary { get; set; } = string.Empty;
}
