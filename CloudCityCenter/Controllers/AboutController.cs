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

                // Отправляем email асинхронно (fire-and-forget), чтобы не блокировать ответ
                // Это предотвращает 504 Gateway Time-out
                _ = Task.Run(async () =>
                {
                    try
                    {
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
                            _logger.LogInformation($"About form email sent successfully from {Name} ({Email})");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to send about form email from {Name} ({Email})");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending about form email from {Name} ({Email})");
                    }
                });

                // Сразу возвращаем успешный ответ, не дожидаясь отправки email
                TempData["Success"] = "Сообщение успешно отправлено! Мы свяжемся с вами в ближайшее время.";
                _logger.LogInformation($"About form submitted from {Name} ({Email}) - email sending in background");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing about form from {Name} ({Email})");
                TempData["Error"] = "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.";
            }

            return RedirectToAction("Index");
        }
}
