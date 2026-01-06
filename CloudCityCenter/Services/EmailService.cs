using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudCityCenter.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string? fromName = null, string? fromEmail = null)
    {
        try
        {
            // Настройки SMTP Hostinger (читаются из переменных окружения или appsettings)
            // Переменные окружения: Email__SmtpHost, Email__SmtpPort, Email__SmtpUsername, Email__SmtpPassword
            var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.hostinger.com";
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 465); // Hostinger использует порт 465
            var smtpUsername = _configuration["Email:SmtpUsername"] ?? "support@cloudcity.center";
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var useSsl = _configuration.GetValue<bool>("Email:UseSsl", true);
            var recipientEmail = _configuration["Email:RecipientEmail"] ?? "support@cloudcity.center";

            // Детальное логирование конфигурации (без пароля)
            _logger.LogInformation($"Email configuration: Host={smtpHost}, Port={smtpPort}, Username={smtpUsername}, SSL={useSsl}, PasswordSet={!string.IsNullOrEmpty(smtpPassword)}");

            // Проверка наличия обязательных параметров
            if (string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogError("Email password not configured. Set Email__SmtpPassword environment variable.");
                _logger.LogError("Available Email config keys: {Keys}", string.Join(", ", 
                    _configuration.GetSection("Email").GetChildren().Select(c => c.Key)));
                return false;
            }

            // Если не указан получатель в параметре, используем из конфигурации
            if (string.IsNullOrEmpty(toEmail))
            {
                toEmail = recipientEmail;
            }

            var message = new MimeMessage();
            
            // Отправитель
            if (!string.IsNullOrEmpty(fromEmail) && !string.IsNullOrEmpty(fromName))
            {
                message.From.Add(new MailboxAddress(fromName, fromEmail));
            }
            else if (!string.IsNullOrEmpty(smtpUsername))
            {
                message.From.Add(new MailboxAddress("CloudCity Center", smtpUsername));
            }
            else
            {
                message.From.Add(new MailboxAddress("CloudCity Center", "noreply@cloudcity.center"));
            }

            // Получатель
            message.To.Add(new MailboxAddress("Support", toEmail));

            // Тема и тело
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            // Отправка через SMTP Hostinger
            using var client = new SmtpClient();
            
            _logger.LogInformation($"Connecting to SMTP server: {smtpHost}:{smtpPort}");
            
            // Hostinger использует порт 465 с SSL (не StartTLS)
            SecureSocketOptions sslOption;
            if (smtpPort == 465)
            {
                sslOption = SecureSocketOptions.SslOnConnect; // Прямое SSL соединение
                _logger.LogInformation("Using SSL on connect (port 465)");
            }
            else
            {
                sslOption = useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                _logger.LogInformation($"Using {(useSsl ? "StartTLS" : "No SSL")} (port {smtpPort})");
            }
            
            // Для диагностики можно временно отключить проверку сертификата
            // ВНИМАНИЕ: Только для тестирования! В продакшене должно быть true
            var checkCertificate = _configuration.GetValue<bool>("Email:CheckCertificate", true);
            if (!checkCertificate)
            {
                _logger.LogWarning("SSL certificate validation is DISABLED. This should only be used for testing!");
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            }
            
            await client.ConnectAsync(smtpHost, smtpPort, sslOption);
            _logger.LogInformation("SMTP connection established");
            
            if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
            {
                _logger.LogInformation($"Authenticating as {smtpUsername}");
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                _logger.LogInformation("SMTP authentication successful");
            }
            
            _logger.LogInformation($"Sending email to {toEmail} with subject: {subject}");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation("SMTP connection closed");

            _logger.LogInformation($"Email sent successfully to {toEmail}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {toEmail}. Error: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                _logger.LogError($"Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }

    public async Task<bool> SendContactFormEmailAsync(string name, string email, string? phone, string? subject, string? serviceType, string message, string sourcePage)
    {
        var emailSubject = $"Новая заявка с {sourcePage} - {subject ?? "Без темы"}";
        
        var emailBody = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #0b1c49; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9f9f9; padding: 20px; }}
        .field {{ margin-bottom: 15px; }}
        .label {{ font-weight: bold; color: #0b1c49; }}
        .value {{ margin-top: 5px; padding: 10px; background-color: white; border-left: 3px solid #0b1c49; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h2>Новая заявка с сайта CloudCity Center</h2>
        </div>
        <div class=""content"">
            <div class=""field"">
                <div class=""label"">Страница:</div>
                <div class=""value"">{sourcePage}</div>
            </div>
            <div class=""field"">
                <div class=""label"">Имя:</div>
                <div class=""value"">{name}</div>
            </div>
            <div class=""field"">
                <div class=""label"">Email:</div>
                <div class=""value"">{email}</div>
            </div>
            {(string.IsNullOrEmpty(phone) ? "" : $@"
            <div class=""field"">
                <div class=""label"">Телефон:</div>
                <div class=""value"">{phone}</div>
            </div>")}
            {(string.IsNullOrEmpty(subject) ? "" : $@"
            <div class=""field"">
                <div class=""label"">Тема:</div>
                <div class=""value"">{subject}</div>
            </div>")}
            {(string.IsNullOrEmpty(serviceType) ? "" : $@"
            <div class=""field"">
                <div class=""label"">Тип услуги:</div>
                <div class=""value"">{serviceType}</div>
            </div>")}
            <div class=""field"">
                <div class=""label"">Сообщение:</div>
                <div class=""value"">{message.Replace("\n", "<br>")}</div>
            </div>
        </div>
    </div>
</body>
</html>";

        return await SendEmailAsync("support@cloudcity.center", emailSubject, emailBody, name, email);
    }
}

