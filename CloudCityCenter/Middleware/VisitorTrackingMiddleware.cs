using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Services;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Middleware;

public class VisitorTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<VisitorTrackingMiddleware> _logger;

    public VisitorTrackingMiddleware(RequestDelegate next, ILogger<VisitorTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var normalizedIp = ClientIpResolver.ResolveNormalizedIp(context);

        if (!string.IsNullOrWhiteSpace(normalizedIp))
        {
            var nowUtc = DateTime.UtcNow;

            try
            {
                var existingSession = await dbContext.VisitorSessions
                    .FirstOrDefaultAsync(x => x.IpAddress == normalizedIp);

                if (existingSession is null)
                {
                    dbContext.VisitorSessions.Add(new VisitorSession
                    {
                        IpAddress = normalizedIp,
                        FirstSeenAt = nowUtc,
                        LastSeenAt = nowUtc
                    });
                }
                else
                {
                    existingSession.LastSeenAt = nowUtc;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Visitor tracking failed for IP {IpAddress} and path {Path}.",
                    normalizedIp,
                    context.Request.Path);
            }
        }

        await _next(context);
    }
}
