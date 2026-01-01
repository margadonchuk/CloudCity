using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Data;
using CloudCityCenter.Models;

namespace CloudCityCenter.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Products
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .OrderBy(p => p.Name)
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

        if (ModelState.IsValid)
        {
            try
            {
                // Проверяем уникальность slug
                var productWithSameSlug = await _context.Products
                    .FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);
                    
                if (productWithSameSlug != null)
                {
                    ModelState.AddModelError("Slug", "Товар с таким slug уже существует.");
                    return View(product);
                }

                // Получаем существующий товар из базы
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Обновляем свойства
                existingProduct.Name = product.Name;
                existingProduct.Slug = product.Slug;
                existingProduct.Type = product.Type;
                existingProduct.Location = product.Location;
                existingProduct.PricePerMonth = product.PricePerMonth;
                existingProduct.Configuration = product.Configuration;
                existingProduct.IsAvailable = product.IsAvailable;
                existingProduct.IsPublished = product.IsPublished;
                existingProduct.ImageUrl = product.ImageUrl;

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(product);
    }

    // GET: Admin/Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
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
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}

