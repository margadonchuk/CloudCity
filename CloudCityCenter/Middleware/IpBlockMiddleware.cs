using System.Net;
using System.Data;
using CloudCityCenter.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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

        if (TryNormalizeIp(context.Connection.RemoteIpAddress, out var normalizedIp))
        {
            var isBlocked = await dbContext.BlockedIps
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
            await using var connection = dbContext.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
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
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Skipping IP block check because BlockedIps table availability could not be verified.");
            _memoryCache.Set(BlockedIpsTableExistsCacheKey, false, TimeSpan.FromSeconds(30));
            return false;
        }
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
