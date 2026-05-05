using System.Net;

namespace CloudCityCenter.Services;

public static class ClientIpResolver
{
    public static string? ResolveNormalizedIp(HttpContext context)
    {
        var candidates = new[]
        {
            context.Request.Headers["CF-Connecting-IP"].FirstOrDefault(),
            GetFirstForwardedFor(context.Request.Headers["X-Forwarded-For"].FirstOrDefault()),
            context.Connection.RemoteIpAddress?.ToString()
        };

        foreach (var candidate in candidates)
        {
            if (TryNormalizeIp(candidate, out var normalizedIp))
            {
                return normalizedIp;
            }
        }

        return null;
    }

    public static bool TryNormalizeIp(string? rawIp, out string normalizedIp)
    {
        normalizedIp = string.Empty;
        if (string.IsNullOrWhiteSpace(rawIp))
        {
            return false;
        }

        return IPAddress.TryParse(rawIp.Trim(), out var parsedIp) && TryNormalizeIp(parsedIp, out normalizedIp);
    }

    public static bool TryNormalizeIp(IPAddress? ipAddress, out string normalizedIp)
    {
        normalizedIp = string.Empty;
        if (ipAddress == null)
        {
            return false;
        }

        if (ipAddress.IsIPv4MappedToIPv6)
        {
            ipAddress = ipAddress.MapToIPv4();
        }

        normalizedIp = ipAddress.ToString();
        return true;
    }

    private static string? GetFirstForwardedFor(string? forwardedFor)
    {
        if (string.IsNullOrWhiteSpace(forwardedFor))
        {
            return null;
        }

        return forwardedFor
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
    }
}
