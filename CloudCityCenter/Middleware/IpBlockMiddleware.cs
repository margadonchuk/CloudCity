using System.Net;
using CloudCityCenter.Data;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Middleware;

public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockMiddleware> _logger;

    public IpBlockMiddleware(RequestDelegate next, ILogger<IpBlockMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (TryNormalizeIp(context.Connection.RemoteIpAddress, out var normalizedIp))
        {
            var isBlocked = await dbContext.BlockedIpAddresses
                .AsNoTracking()
                .AnyAsync(x => x.IsActive && x.NormalizedIpAddress == normalizedIp);

            if (isBlocked)
            {
                _logger.LogWarning("Blocked request from IP {IpAddress} to {Path}", normalizedIp, context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }

    private static bool TryNormalizeIp(IPAddress? ipAddress, out string normalizedIp)
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
}
