using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;

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
            
            // Отправитель - ВСЕГДА используем SMTP username (support@cloudcity.center)
            // Hostinger не позволяет отправлять от другого адреса
            if (!string.IsNullOrEmpty(smtpUsername))
            {
                message.From.Add(new MailboxAddress("CloudCity Center", smtpUsername));
            }
            else
            {
                message.From.Add(new MailboxAddress("CloudCity Center", "support@cloudcity.center"));
            }

            // Reply-To - здесь указываем email пользователя, если он указан
            if (!string.IsNullOrEmpty(fromEmail))
            {
                message.ReplyTo.Add(new MailboxAddress(fromName ?? "User", fromEmail));
                _logger.LogInformation($"Reply-To set to: {fromEmail}");
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
            
            // Hostinger может использовать порт 465 (SSL) или 587 (StartTLS)
            // Пробуем разные варианты в зависимости от порта
            SecureSocketOptions sslOption;
            if (smtpPort == 465)
            {
                // Порт 465 требует прямого SSL соединения
                sslOption = SecureSocketOptions.SslOnConnect;
                _logger.LogInformation("Using SSL on connect (port 465)");
            }
            else if (smtpPort == 587)
            {
                // Порт 587 использует StartTLS (сначала обычное соединение, потом SSL)
                sslOption = SecureSocketOptions.StartTls;
                _logger.LogInformation("Using StartTLS (port 587)");
            }
            else if (smtpPort == 25)
            {
                // Порт 25 обычно без SSL (не рекомендуется, но может работать)
                sslOption = SecureSocketOptions.None;
                _logger.LogWarning("Using port 25 without SSL - not recommended!");
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
            
            try
            {
                _logger.LogInformation($"Attempting to connect to {smtpHost}:{smtpPort}...");
                
                // Устанавливаем таймаут для подключения (30 секунд)
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                
                await client.ConnectAsync(smtpHost, smtpPort, sslOption, cts.Token);
                _logger.LogInformation($"SMTP connection established. IsConnected: {client.IsConnected}, IsAuthenticated: {client.IsAuthenticated}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogError($"Connection to {smtpHost}:{smtpPort} timed out after 30 seconds");
                throw new TimeoutException($"SMTP connection timeout to {smtpHost}:{smtpPort}");
            }
            catch (Exception connectEx)
            {
                _logger.LogError(connectEx, $"Failed to connect to SMTP server {smtpHost}:{smtpPort}. Error: {connectEx.Message}");
                _logger.LogError($"Connection error type: {connectEx.GetType().Name}, Inner: {connectEx.InnerException?.Message}");
                throw;
            }
            
            try
            {
                if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogInformation($"Authenticating as {smtpUsername}...");
                    
                    // Таймаут для аутентификации (15 секунд)
                    using var authCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    
                    await client.AuthenticateAsync(smtpUsername, smtpPassword, authCts.Token);
                    _logger.LogInformation($"SMTP authentication successful. IsAuthenticated: {client.IsAuthenticated}");
                }
                else
                {
                    _logger.LogWarning("SMTP username or password is empty, skipping authentication");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError($"SMTP authentication timed out for {smtpUsername}");
                await client.DisconnectAsync(true);
                throw new TimeoutException($"SMTP authentication timeout");
            }
            catch (Exception authEx)
            {
                _logger.LogError(authEx, $"SMTP authentication failed for {smtpUsername}. Error: {authEx.Message}");
                _logger.LogError($"Auth error type: {authEx.GetType().Name}, Inner: {authEx.InnerException?.Message}");
                await client.DisconnectAsync(true);
                throw;
            }
            
            try
            {
                _logger.LogInformation($"Sending email to {toEmail} with subject: {subject}");
                
                // Таймаут для отправки (30 секунд)
                using var sendCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                
                var response = await client.SendAsync(message, sendCts.Token);
                _logger.LogInformation($"Email sent successfully. Response: {response}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogError($"Sending email timed out");
                throw new TimeoutException("SMTP send timeout");
            }
            catch (Exception sendEx)
            {
                _logger.LogError(sendEx, $"Failed to send email. Error: {sendEx.Message}");
                _logger.LogError($"Send error type: {sendEx.GetType().Name}, Inner: {sendEx.InnerException?.Message}");
                throw;
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                    _logger.LogInformation("SMTP connection closed");
                }
            }

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

