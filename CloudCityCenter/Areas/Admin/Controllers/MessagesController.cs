using System;
using System.Collections.Generic;
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
public class MessagesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ApplicationDbContext context, ILogger<MessagesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: Admin/Messages
    public async Task<IActionResult> Index(string? sourcePage = null, bool? unreadOnly = null)
    {
        try
        {
            // Проверяем, существует ли таблица
            if (!await _context.Database.CanConnectAsync())
            {
                _logger.LogError("Cannot connect to database");
                ViewBag.Error = "Не удалось подключиться к базе данных. Проверьте строку подключения.";
                return View(Enumerable.Empty<ContactMessage>());
            }

            var query = _context.ContactMessages.AsNoTracking().AsQueryable();

        // Фильтр по странице источника
        if (!string.IsNullOrEmpty(sourcePage))
        {
            query = query.Where(m => m.SourcePage == sourcePage);
        }

        // Фильтр по непрочитанным
        if (unreadOnly == true)
        {
            query = query.Where(m => !m.IsRead);
        }

        // Сортировка: сначала непрочитанные, потом по дате (новые сверху)
        var messages = await query
            .OrderByDescending(m => m.IsRead)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();

            ViewBag.SourcePage = sourcePage;
            ViewBag.UnreadOnly = unreadOnly;
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            return View(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading messages. Table might not exist. Apply migration: dotnet ef database update");
            ViewBag.Error = $"Ошибка загрузки писем: {ex.Message}. Возможно, миграция не применена. Выполните: dotnet ef database update";
            return View(Enumerable.Empty<ContactMessage>());
        }
    }

    // GET: Admin/Messages/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var message = await _context.ContactMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return NotFound();
        }

        // Помечаем как прочитанное, если еще не прочитано
        if (!message.IsRead)
        {
            var messageToUpdate = await _context.ContactMessages.FindAsync(id);
            if (messageToUpdate != null)
            {
                messageToUpdate.IsRead = true;
                messageToUpdate.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Message {id} marked as read");
            }
        }

        return View(message);
    }

    // POST: Admin/Messages/MarkAsRead/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
        {
            return NotFound();
        }

        if (!message.IsRead)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Message {id} marked as read");
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Messages/MarkAsUnread/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsUnread(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message == null)
        {
            return NotFound();
        }

        message.IsRead = false;
        message.ReadAt = null;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Message {id} marked as unread");

        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Messages/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var message = await _context.ContactMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return NotFound();
        }

        return View(message);
    }

    // POST: Admin/Messages/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message != null)
        {
            _context.ContactMessages.Remove(message);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Message {MessageId} deleted", id);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Messages/BulkDelete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkDelete(List<int>? selectedMessageIds, string? sourcePage = null, bool? unreadOnly = null)
    {
        if (selectedMessageIds == null || selectedMessageIds.Count == 0)
        {
            TempData["Error"] = "Please select at least one message.";
            return RedirectToAction(nameof(Index), new { sourcePage, unreadOnly });
        }

        var normalizedIds = selectedMessageIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (normalizedIds.Count == 0)
        {
            TempData["Error"] = "Please select at least one message.";
            return RedirectToAction(nameof(Index), new { sourcePage, unreadOnly });
        }

        var messagesToDelete = await _context.ContactMessages
            .Where(m => normalizedIds.Contains(m.Id))
            .ToListAsync();

        if (messagesToDelete.Count == 0)
        {
            TempData["Error"] = "No selected messages were found.";
            return RedirectToAction(nameof(Index), new { sourcePage, unreadOnly });
        }

        _context.ContactMessages.RemoveRange(messagesToDelete);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Bulk deleted {DeletedCount} messages", messagesToDelete.Count);
        TempData["Success"] = "Selected messages deleted successfully";

        return RedirectToAction(nameof(Index), new { sourcePage, unreadOnly });
    }
}

