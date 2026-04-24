namespace CloudCityCenter.Models.Admin;

public sealed class BlockedIpsIndexViewModel
{
    public IReadOnlyList<BlockedIpListItemViewModel> Items { get; init; } = Array.Empty<BlockedIpListItemViewModel>();

    public bool IsFeatureInitialized { get; init; } = true;

    public string? FeatureMessage { get; init; }
}
