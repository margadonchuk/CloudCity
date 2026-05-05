using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Services;
using Microsoft.EntityFrameworkCore;

namespace CloudCityCenter.Middleware;

public class VisitorTrackingMiddleware
{
    private static readonly string[] StaticExtensions =
    [
        ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".webp", ".avif", ".bmp", ".woff", ".woff2", ".ttf", ".eot", ".otf"
    ];

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
                    existingSession = new VisitorSession
                    {
                        IpAddress = normalizedIp,
                        FirstSeenAt = nowUtc,
                        LastSeenAt = nowUtc
                    };
                    dbContext.VisitorSessions.Add(existingSession);
                }
                else
                {
                    existingSession.LastSeenAt = nowUtc;
                }

                if (ShouldTrackPageVisit(context.Request.Path))
                {
                    dbContext.PageVisits.Add(new PageVisit
                    {
                        VisitorSession = existingSession,
                        Path = context.Request.Path.Value ?? "/",
                        QueryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                        HttpMethod = context.Request.Method,
                        VisitedAt = nowUtc
                    });
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

    private static bool ShouldTrackPageVisit(PathString path)
    {
        var value = path.Value ?? string.Empty;

        if (string.IsNullOrEmpty(value) || value.Equals("/", StringComparison.Ordinal))
        {
            return true;
        }

        if (value.StartsWith("/Admin/Analytics", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !StaticExtensions.Any(ext => value.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
}
