using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ServersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ServersController> _logger;

    public ServersController(ApplicationDbContext context, ILogger<ServersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Servers
    public async Task<IActionResult> Index()
    {
        var servers = await _context.Servers.AsNoTracking().ToListAsync();
        return View(servers);
    }

    // GET: Admin/Servers/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // GET: Admin/Servers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Servers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Slug,Description,Location,PricePerMonth,CPU,RamGb,StorageGb,ImageUrl,IsActive,DDoSTier,Stock")] Server server)
    {
        if (!ModelState.IsValid)
        {
            return View(server);
        }

        if (await _context.Servers.AnyAsync(s => s.Slug == server.Slug) ||
            await _context.Products.AnyAsync(p => p.Slug == server.Slug))
        {
            ModelState.AddModelError("Slug", "Slug already exists.");
            return View(server);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Add(server);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Server saved to database with ID: {ServerId}", server.Id);

            var product = new Product
            {
                Name = server.Name,
                Slug = server.Slug,
                Type = ProductType.DedicatedServer,
                Location = server.Location,
                PricePerMonth = server.PricePerMonth,
                Configuration = $"{server.CPU}, {server.RamGb} GB RAM, {server.StorageGb} GB storage",
                ImageUrl = server.ImageUrl,
                IsAvailable = server.IsActive,
                IsPublished = server.IsActive,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product saved to database with ID: {ProductId}", product.Id);

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for server {ServerSlug}", server.Slug);

            _logger.LogInformation("Server {ServerSlug} created successfully", server.Slug);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Database update error while creating server {ServerSlug}. Transaction rolled back.", server.Slug);
            
            if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                ex.InnerException?.Message.Contains("IX_Servers_Slug") == true ||
                ex.InnerException?.Message.Contains("IX_Products_Slug") == true)
            {
                ModelState.AddModelError("Slug", "Slug already exists.");
                return View(server);
            }

            _logger.LogError(ex, "Unhandled database exception: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Unexpected error while creating server {ServerSlug}. Transaction rolled back.", server.Slug);
            throw;
        }
    }

    // GET: Admin/Servers/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.FindAsync(id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // POST: Admin/Servers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Description,Location,PricePerMonth,CPU,RamGb,StorageGb,ImageUrl,IsActive,DDoSTier,Stock")] Server server)
    {
        if (id != server.Id)
        {
            return NotFound();
        }

        var existingServer = await _context.Servers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (existingServer == null)
        {
            return NotFound();
        }

        var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == existingServer.Slug);

        if (!ModelState.IsValid)
        {
            return View(server);
        }

        bool slugExistsInServers = await _context.Servers.AnyAsync(s => s.Slug == server.Slug && s.Id != server.Id);
        bool slugExistsInProducts = existingProduct == null
            ? await _context.Products.AnyAsync(p => p.Slug == server.Slug)
            : await _context.Products.AnyAsync(p => p.Slug == server.Slug && p.Id != existingProduct.Id);

        if (slugExistsInServers || slugExistsInProducts)
        {
            ModelState.AddModelError("Slug", "Slug already exists.");
            return View(server);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Update(server);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Server updated in database with ID: {ServerId}", server.Id);

            if (existingProduct == null)
            {
                var product = new Product
                {
                    Name = server.Name,
                    Slug = server.Slug,
                    Type = ProductType.DedicatedServer,
                    Location = server.Location,
                    PricePerMonth = server.PricePerMonth,
                    Configuration = $"{server.CPU}, {server.RamGb} GB RAM, {server.StorageGb} GB storage",
                    ImageUrl = server.ImageUrl,
                    IsAvailable = server.IsActive,
                    IsPublished = server.IsActive,
                };
                _context.Products.Add(product);
                _logger.LogInformation("New product created for server {ServerSlug}", server.Slug);
            }
            else
            {
                existingProduct.Name = server.Name;
                existingProduct.Slug = server.Slug;
                existingProduct.Location = server.Location;
                existingProduct.PricePerMonth = server.PricePerMonth;
                existingProduct.Configuration = $"{server.CPU}, {server.RamGb} GB RAM, {server.StorageGb} GB storage";
                existingProduct.ImageUrl = server.ImageUrl;
                existingProduct.IsAvailable = server.IsActive;
                existingProduct.IsPublished = server.IsActive;
                _context.Update(existingProduct);
                _logger.LogInformation("Existing product updated for server {ServerSlug}", server.Slug);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Product changes saved to database");

            await transaction.CommitAsync();
            _logger.LogInformation("Transaction committed successfully for server {ServerSlug}", server.Slug);

            _logger.LogInformation("Server {ServerSlug} edited successfully", server.Slug);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Database update error while editing server {ServerSlug}. Transaction rolled back.", server.Slug);
            
            if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                ex.InnerException?.Message.Contains("IX_Servers_Slug") == true ||
                ex.InnerException?.Message.Contains("IX_Products_Slug") == true)
            {
                ModelState.AddModelError("Slug", "Slug already exists.");
                return View(server);
            }

            _logger.LogError(ex, "Unhandled database exception: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Unexpected error while editing server {ServerSlug}. Transaction rolled back.", server.Slug);
            throw;
        }
    }

    // GET: Admin/Servers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var server = await _context.Servers.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        if (server == null)
        {
            return NotFound();
        }

        return View(server);
    }

    // POST: Admin/Servers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var server = await _context.Servers.FindAsync(id);
        if (server != null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Slug == server.Slug);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _logger.LogInformation("Product {ProductSlug} marked for deletion", product.Slug);
                }

                _context.Servers.Remove(server);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Server {ServerSlug} removed from database", server.Slug);

                await transaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully for server deletion {ServerSlug}", server.Slug);

                _logger.LogInformation("Server {ServerSlug} deleted successfully", server.Slug);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error while deleting server {ServerSlug}. Transaction rolled back.", server.Slug);
                throw;
            }
        }
        return RedirectToAction(nameof(Index));
    }
}

