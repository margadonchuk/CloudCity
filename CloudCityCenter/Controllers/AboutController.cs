using Microsoft.AspNetCore.Mvc;
using CloudCityCenter.Services;

namespace CloudCityCenter.Controllers;

public class AboutController : Controller
{
    private readonly EmailService _emailService;
    private readonly ILogger<AboutController> _logger;

    public AboutController(EmailService emailService, ILogger<AboutController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(string Name, string Email, string Subject, string Message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
            {
                TempData["Error"] = "Пожалуйста, заполните все обязательные поля.";
                return RedirectToAction("Index");
            }

            var success = await _emailService.SendContactFormEmailAsync(
                name: Name,
                email: Email,
                phone: null,
                subject: Subject,
                serviceType: null,
                message: Message,
                sourcePage: "About"
            );

            if (success)
            {
                TempData["Success"] = "Сообщение успешно отправлено! Мы свяжемся с вами в ближайшее время.";
                _logger.LogInformation($"About form submitted successfully from {Name} ({Email})");
            }
            else
            {
                TempData["Error"] = "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.";
                _logger.LogWarning($"Failed to send about form email from {Name} ({Email})");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing about form from {Name} ({Email})");
            TempData["Error"] = "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.";
        }

        return RedirectToAction("Index");
    }
}
