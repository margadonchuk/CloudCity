using System.Net;
using System.Text.Json;

namespace CloudCityCenter.Services;

public sealed class IpApiGeoIpService : IGeoIpService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IpApiGeoIpService> _logger;

    public IpApiGeoIpService(HttpClient httpClient, ILogger<IpApiGeoIpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GeoIpLookupResult?> LookupAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (!TryParsePublicIp(ipAddress, out _))
        {
            return null;
        }

        try
        {
            using var response = await _httpClient.GetAsync($"json/{ipAddress}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GeoIP lookup failed for {IpAddress}. Status code: {StatusCode}", ipAddress, response.StatusCode);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var payload = await JsonSerializer.DeserializeAsync<IpApiResponse>(stream, cancellationToken: cancellationToken);
            if (payload is null || !string.Equals(payload.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("GeoIP lookup returned unsuccessful payload for {IpAddress}. Status: {Status}", ipAddress, payload?.Status);
                return null;
            }

            return new GeoIpLookupResult(
                Clean(payload.Country),
                Clean(payload.CountryCode),
                Clean(payload.City));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GeoIP lookup threw exception for {IpAddress}", ipAddress);
            return null;
        }
    }

    private static bool TryParsePublicIp(string ipAddress, out IPAddress parsedIp)
    {
        parsedIp = IPAddress.None;
        if (!IPAddress.TryParse(ipAddress, out parsedIp))
        {
            return false;
        }

        if (parsedIp.IsIPv4MappedToIPv6)
        {
            parsedIp = parsedIp.MapToIPv4();
        }

        if (IPAddress.IsLoopback(parsedIp))
        {
            return false;
        }

        if (parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return !parsedIp.IsIPv6LinkLocal && !parsedIp.IsIPv6SiteLocal && !parsedIp.IsIPv6Multicast;
        }

        var bytes = parsedIp.GetAddressBytes();
        if (bytes[0] == 10)
        {
            return false;
        }

        if (bytes[0] == 192 && bytes[1] == 168)
        {
            return false;
        }

        return !(bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31);
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed class IpApiResponse
    {
        public string? Status { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? City { get; set; }
    }
}
