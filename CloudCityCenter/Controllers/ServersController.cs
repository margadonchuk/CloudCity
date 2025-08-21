using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;
using CloudCityCenter.Models.ViewModels;
using System.Collections.Generic;
using System;

namespace CloudCityCenter.Controllers;

/// <summary>
/// Public controller for browsing available servers.
/// </summary>
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lists active servers with optional filtering, sorting and pagination.
    /// </summary>
    public async Task<IActionResult> Index(
        string? location,
        int? minRam,
        int? maxRam,
        string? q,
        string? sort,
        int page = 1,
        int pageSize = 12)
    {
        pageSize = Math.Clamp(pageSize, 1, 48);
        if (page < 1) page = 1;

        var query = _context.Servers
            .AsNoTracking()
            .Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(s => s.Location == location);
        }

        if (minRam.HasValue)
        {
            query = query.Where(s => s.RamGb >= minRam.Value);
        }

        if (maxRam.HasValue)
        {
            query = query.Where(s => s.RamGb <= maxRam.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(s =>
                s.Name.Contains(q) || (s.Description != null && s.Description.Contains(q)));
        }

        query = sort switch
        {
            "price_asc" => query.OrderBy(s => s.PricePerMonth),
            "price_desc" => query.OrderByDescending(s => s.PricePerMonth),
            "newest" => query.OrderByDescending(s => s.CreatedUtc),
            _ => query.OrderBy(s => s.Id)
        };

        var totalCount = await query.CountAsync();

        var servers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new ServerCardVm
            {
                ImageUrl = s.ImageUrl,
                Name = s.Name,
                CpuCores = s.CpuCores,
                RamGb = s.RamGb,
                StorageGb = s.StorageGb,
                Location = s.Location,
                PricePerMonth = s.PricePerMonth,
                Slug = s.Slug
            })
            .ToListAsync();

        var filters = new List<string>();
        if (!string.IsNullOrWhiteSpace(location))
        {
            filters.Add($"Location: {location}");
        }
        if (minRam.HasValue)
        {
            filters.Add($"Min RAM: {minRam} GB");
        }
        if (maxRam.HasValue)
        {
            filters.Add($"Max RAM: {maxRam} GB");
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            filters.Add($"Search: {q}");
        }

        var vm = new ServerIndexViewModel
        {
            Servers = servers,
            Location = location,
            MinRam = minRam,
            MaxRam = maxRam,
            Q = q,
            Sort = sort,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            FiltersSummary = filters.Count == 0 ? "None" : string.Join(", ", filters)
        };

        return View(vm);
    }

    /// <summary>
    /// Shows details for a server identified by slug.
    /// </summary>
    public async Task<IActionResult> Details(string? slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        var server = await _context.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Slug == slug && s.IsActive);

        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }
}

