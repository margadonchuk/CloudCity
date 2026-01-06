using Microsoft.AspNetCore.Mvc;
using CloudCityCenter.Services;

namespace CloudCityCenter.Controllers
{
    public class ContactController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(EmailService emailService, ILogger<ContactController> logger)
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
        public async Task<IActionResult> Submit(string Name, string Email, string Phone, string ServiceType, string Message)
        {
            _logger.LogInformation($"Contact form Submit called. Name: {Name}, Email: {Email}, Phone: {Phone}, Message length: {Message?.Length ?? 0}");
            
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
                            phone: Phone,
                            subject: null,
                            serviceType: ServiceType,
                            message: Message,
                            sourcePage: "Contact"
                        );

                        if (success)
                        {
                            _logger.LogInformation($"Contact form email sent successfully from {Name} ({Email})");
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to send contact form email from {Name} ({Email})");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending contact form email from {Name} ({Email})");
                    }
                });

                // Сразу возвращаем успешный ответ, не дожидаясь отправки email
                TempData["Success"] = "Сообщение успешно отправлено! Мы свяжемся с вами в ближайшее время.";
                _logger.LogInformation($"Contact form submitted from {Name} ({Email}) - email sending in background");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing contact form from {Name} ({Email})");
                TempData["Error"] = "Произошла ошибка при отправке сообщения. Пожалуйста, попробуйте позже.";
            }

            return RedirectToAction("Index");
        }
    }
}
