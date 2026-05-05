namespace CloudCityCenter.Services;

public sealed record GeoIpLookupResult(string? Country, string? CountryCode, string? City);
