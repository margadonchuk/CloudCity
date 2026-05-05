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
    private readonly IGeoIpService _geoIpService;

    public VisitorTrackingMiddleware(RequestDelegate next, ILogger<VisitorTrackingMiddleware> logger, IGeoIpService geoIpService)
    {
        _next = next;
        _logger = logger;
        _geoIpService = geoIpService;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var normalizedIp = ClientIpResolver.ResolveNormalizedIp(context);

        if (!string.IsNullOrWhiteSpace(normalizedIp))
        {
            var nowUtc = DateTime.UtcNow;
            var userAgent = ResolveUserAgent(context);
            var referrer = ResolveReferrer(context);

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
                        LastSeenAt = nowUtc,
                        UserAgent = userAgent,
                        Referrer = referrer
                    };
                    dbContext.VisitorSessions.Add(existingSession);
                }
                else
                {
                    existingSession.LastSeenAt = nowUtc;

                    if (!string.IsNullOrWhiteSpace(userAgent))
                    {
                        existingSession.UserAgent = userAgent;
                    }

                    if (!string.IsNullOrWhiteSpace(referrer))
                    {
                        existingSession.Referrer = referrer;
                    }
                }

                if (string.IsNullOrWhiteSpace(existingSession.Country))
                {
                    var geo = await _geoIpService.LookupAsync(normalizedIp, context.RequestAborted);
                    if (geo is not null)
                    {
                        existingSession.Country = geo.Country;
                        existingSession.CountryCode = geo.CountryCode;
                        existingSession.City = geo.City;
                    }
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

    private static string? ResolveUserAgent(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return null;
        }

        return userAgent.Length <= 1024
            ? userAgent
            : userAgent[..1024];
    }


    private static string? ResolveReferrer(HttpContext context)
    {
        var referrer = context.Request.Headers["Referer"].ToString();
        if (string.IsNullOrWhiteSpace(referrer))
        {
            return null;
        }

        return referrer.Length <= 2048
            ? referrer
            : referrer[..2048];
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
