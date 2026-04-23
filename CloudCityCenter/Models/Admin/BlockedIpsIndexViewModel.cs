using CloudCityCenter.Models;

namespace CloudCityCenter.Models.Admin;

public sealed class BlockedIpsIndexViewModel
{
    public IReadOnlyList<BlockedIp> Items { get; init; } = Array.Empty<BlockedIp>();

    public bool IsFeatureInitialized { get; init; } = true;

    public string? FeatureMessage { get; init; }
}
