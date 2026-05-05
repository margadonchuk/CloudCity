namespace CloudCityCenter.Services;

public interface IGeoIpService
{
    Task<GeoIpLookupResult?> LookupAsync(string ipAddress, CancellationToken cancellationToken = default);
}
