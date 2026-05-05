using System.Data;
using System.Data.Common;
using CloudCityCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using CloudCityCenter.Services;

namespace CloudCityCenter.Middleware;

public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockMiddleware> _logger;
    private readonly IMemoryCache _memoryCache;
    private const string BlockedIpsTableExistsCacheKey = "blocked_ips_table_exists";

    public IpBlockMiddleware(RequestDelegate next, ILogger<IpBlockMiddleware> logger, IMemoryCache memoryCache)
    {
        _next = next;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        if (!await IsBlockedIpsTableAvailableAsync(dbContext))
        {
            await _next(context);
            return;
        }

        var normalizedIp = ClientIpResolver.ResolveNormalizedIp(context);
        _logger.LogInformation("Detected client IP {IpAddress} for request {Path}", normalizedIp ?? "(unknown)", context.Request.Path);

        if (!ShouldCheckBlocking(context))
        {
            await _next(context);
            return;
        }

        if (!string.IsNullOrEmpty(normalizedIp))
        {
            bool isBlocked;

            try
            {
                isBlocked = await dbContext.BlockedIps
                    .AsNoTracking()
                    .AnyAsync(x => x.IsActive && x.IpAddress == normalizedIp);
            }
            catch (Exception ex) when (IsTransientBlockedIpQueryFailure(ex))
            {
                _logger.LogWarning(ex,
                    "Skipping blocked IP check for {IpAddress} because BlockedIps query failed.",
                    normalizedIp);
                _memoryCache.Set(BlockedIpsTableExistsCacheKey, false, TimeSpan.FromSeconds(30));
                await _next(context);
                return;
            }

            if (isBlocked)
            {
                _logger.LogWarning("Blocked request from IP {IpAddress} to {Path}", normalizedIp, context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }
        }

        await _next(context);
    }

    private async Task<bool> IsBlockedIpsTableAvailableAsync(ApplicationDbContext dbContext)
    {
        if (_memoryCache.TryGetValue<bool>(BlockedIpsTableExistsCacheKey, out var cachedValue))
        {
            return cachedValue;
        }

        if (!dbContext.Database.IsRelational())
        {
            _memoryCache.Set(BlockedIpsTableExistsCacheKey, true, TimeSpan.FromMinutes(5));
            return true;
        }

        try
        {
            var connection = dbContext.Database.GetDbConnection();
            var openedByMiddleware = false;

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
                openedByMiddleware = true;
            }

            await using var command = connection.CreateCommand();
            if (dbContext.Database.IsSqlServer())
            {
                command.CommandText =
                    "SELECT CASE WHEN EXISTS (SELECT 1 FROM sys.tables WHERE name = 'BlockedIps' AND schema_id = SCHEMA_ID('dbo')) THEN 1 ELSE 0 END";
            }
            else if (dbContext.Database.IsSqlite())
            {
                command.CommandText =
                    "SELECT CASE WHEN EXISTS (SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = 'BlockedIps') THEN 1 ELSE 0 END";
            }
            else
            {
                _memoryCache.Set(BlockedIpsTableExistsCacheKey, true, TimeSpan.FromMinutes(5));
                return true;
            }

            var existsResult = await command.ExecuteScalarAsync();
            var exists = Convert.ToInt32(existsResult) == 1;
            _memoryCache.Set(BlockedIpsTableExistsCacheKey, exists, TimeSpan.FromSeconds(30));

            if (openedByMiddleware)
            {
                await connection.CloseAsync();
            }

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Skipping IP block check because BlockedIps table availability could not be verified.");
            _memoryCache.Set(BlockedIpsTableExistsCacheKey, false, TimeSpan.FromSeconds(30));
            return false;
        }
    }

    private static bool ShouldCheckBlocking(HttpContext context)
    {
        var path = context.Request.Path;

        if (path.StartsWithSegments("/css")
            || path.StartsWithSegments("/js")
            || path.StartsWithSegments("/lib")
            || path.StartsWithSegments("/images")
            || path.StartsWithSegments("/favicon.ico")
            || path.StartsWithSegments("/Home/StatusCode"))
        {
            return false;
        }

        if (context.User?.Identity?.IsAuthenticated == true
            && context.User.IsInRole("Admin")
            && path.StartsWithSegments("/admin/security/blockedips"))
        {
            return false;
        }

        return true;
    }

    private static bool IsTransientBlockedIpQueryFailure(Exception exception) =>
        exception is DbException
            or InvalidOperationException
            or TimeoutException;


}
