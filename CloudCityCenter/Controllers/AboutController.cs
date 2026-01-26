using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudCityCenter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CloudCityCenter.Controllers;

public class AboutController : Controller
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AboutController> _logger;
    private readonly FormRateLimitService _formRateLimitService;
    private readonly IStringLocalizerFactory _localizerFactory;

    public AboutController(IServiceScopeFactory serviceScopeFactory, ILogger<AboutController> logger, IStringLocalizerFactory localizerFactory, FormRateLimitService formRateLimitService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _localizerFactory = localizerFactory;
        _formRateLimitService = formRateLimitService;
    }
    
    private IStringLocalizer GetLocalizer()
    {
        return _localizerFactory.Create("Views.About.Index", "CloudCityCenter");
    }

    public IActionResult Index()
    {
        // SEO оптимизация с локализацией
        var localizer = GetLocalizer();
        ViewData["Title"] = localizer["SEOTitle"].Value;
        ViewData["Description"] = localizer["SEODescription"].Value;
        ViewData["Keywords"] = localizer["SEOKeywords"].Value;
        
        return View();
    }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string Name, string Email, string Subject, string Message)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            if (await _formRateLimitService.RegisterSubmissionAsync(ipAddress))
            {
                _logger.LogWarning("Blocked about form submission from IP {IpAddress}", ipAddress);
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Message))
                {
                    TempData["Error"] = "Пожалуйста, заполните все обязательные поля.";
                    return RedirectToAction("Index");
                }

                // Отправляем email асинхронно (fire-and-forget), чтобы не блокировать ответ
                // Это предотвращает 504 Gateway Time-out
                // Создаем новый scope для БД, так как Task.Run выполняется вне scope HTTP-запроса
                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                    
                    try
                    {
                        var success = await emailService.SendContactFormEmailAsync(
                            name: Name,
                            email: Email,
                            phone: null,
                            subject: Subject,
                            serviceType: null,
                            message: Message,
                            sourcePage: "About",
                            ipAddress: ipAddress
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
