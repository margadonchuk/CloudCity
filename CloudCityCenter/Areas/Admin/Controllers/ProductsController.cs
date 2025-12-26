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
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Products
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Features)
            .AsNoTracking()
            .OrderBy(p => p.Type)
            .ThenBy(p => p.Name)
            .ToListAsync();
        return View(products);
    }

    // GET: Admin/Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Features)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: Admin/Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Admin/Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Slug,Type,Location,PricePerMonth,Configuration,IsAvailable,IsPublished,ImageUrl")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        // Проверяем уникальность slug
        var existingProduct = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);
        
        if (existingProduct != null)
        {
            ModelState.AddModelError("Slug", "Slug already exists.");
            return View(product);
        }

        try
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product {ProductId} edited", product.Id);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProductExistsAsync(product.Id))
            {
                return NotFound();
            }
            throw;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                ex.InnerException?.Message.Contains("IX_Products_Slug") == true)
            {
                ModelState.AddModelError("Slug", "Slug already exists.");
                return View(product);
            }
            throw;
        }
    }

    // GET: Admin/Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Features)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // POST: Admin/Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Features)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product != null)
        {
            // Удаляем связанные варианты и характеристики
            _context.ProductVariants.RemoveRange(product.Variants);
            _context.ProductFeatures.RemoveRange(product.Features);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Product {ProductId} deleted", id);
        }
        
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ProductExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(e => e.Id == id);
    }
}

