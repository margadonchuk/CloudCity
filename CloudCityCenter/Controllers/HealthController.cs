using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;

namespace CloudCityCenter.Controllers;

public class HealthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Простая проверка работоспособности приложения
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health()
    {
        return Ok(new 
        { 
            status = "ok", 
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }

    /// <summary>
    /// Проверка работоспособности с проверкой подключения к БД
    /// </summary>
    [HttpGet("/health/detailed")]
    public async Task<IActionResult> HealthDetailed()
    {
        var health = new
        {
            status = "ok",
            timestamp = DateTime.UtcNow,
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            database = "unknown"
        };

        try
        {
            // Проверка подключения к БД
            if (_context.Database.IsRelational())
            {
                var canConnect = await _context.Database.CanConnectAsync();
                health = new
                {
                    status = canConnect ? "ok" : "degraded",
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    database = canConnect ? "connected" : "disconnected"
                };
            }
            else
            {
                // InMemory database
                health = new
                {
                    status = "ok",
                    timestamp = DateTime.UtcNow,
                    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                    database = "in-memory"
                };
            }

            return Ok(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "error",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                database = "error",
                error = ex.Message
            });
        }
    }
}

