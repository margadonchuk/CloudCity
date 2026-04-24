namespace CloudCityCenter.Models.Admin;

public sealed class BlockedIpListItemViewModel
{
    public int Id { get; init; }

    public string IpAddress { get; init; } = string.Empty;

    public string? Reason { get; init; }

    public DateTime? CreatedAtUtc { get; init; }

    public bool IsActive { get; init; }
}
