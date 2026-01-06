# 🔍 Диагностика проблем с Email

## Проверка переменных окружения

На сервере выполните:

```bash
# Проверьте, что переменные окружения установлены
sudo systemctl show cloudcity | grep Email

# Должны быть видны:
# Email__SmtpHost=smtp.hostinger.com
# Email__SmtpPort=465
# Email__SmtpUsername=support@cloudcity.center
# Email__SmtpPassword=*** (скрыто)
```

## Проверка логов

```bash
# Смотрите логи в реальном времени
sudo journalctl -u cloudcity -f

# Или последние 100 строк
sudo journalctl -u cloudcity -n 100
```

## Что искать в логах

### ✅ Успешная отправка:
```
Email configuration: Host=smtp.hostinger.com, Port=465, Username=support@cloudcity.center, SSL=True, PasswordSet=True
Connecting to SMTP server: smtp.hostinger.com:465
Using SSL on connect (port 465)
SMTP connection established
Authenticating as support@cloudcity.center
SMTP authentication successful
Sending email to support@cloudcity.center with subject: ...
Email sent successfully to support@cloudcity.center
```

### ❌ Ошибки:

**1. Пароль не настроен:**
```
Email password not configured. Set Email__SmtpPassword environment variable.
```
**Решение:** Проверьте, что `Email__SmtpPassword` установлен в systemd service файле.

**2. Ошибка подключения:**
```
Failed to send email. Error: Unable to connect to the remote server
```
**Решение:** 
- Проверьте доступность `smtp.hostinger.com`
- Проверьте, что порт 465 не заблокирован файрволом
- Попробуйте: `telnet smtp.hostinger.com 465`

**3. Ошибка аутентификации:**
```
SMTP authentication failed
```
**Решение:**
- Проверьте правильность email и пароля
- Убедитесь, что используете полный email (support@cloudcity.center)
- Проверьте, что почтовый ящик создан в панели Hostinger

**4. SSL ошибка:**
```
The SSL connection could not be established
```
**Решение:**
- Убедитесь, что порт 465 используется с `SslOnConnect`
- Проверьте сертификат SMTP сервера

## Ручная проверка SMTP

### Тест подключения:
```bash
# Проверка доступности порта
nc -zv smtp.hostinger.com 465

# Или с telnet
telnet smtp.hostinger.com 465
```

### Тест через командную строку:
```bash
# Установите mailutils (если нужно)
sudo apt-get install mailutils

# Отправьте тестовое письмо
echo "Test message" | mail -s "Test" support@cloudcity.center
```

## Проверка конфигурации systemd

```bash
# Просмотр полного файла сервиса
sudo systemctl cat cloudcity

# Проверка переменных окружения
sudo systemctl show cloudcity --property=Environment
```

## Типичные проблемы

### 1. Переменные окружения не применяются

**Причина:** Сервис не перезагружен после изменения

**Решение:**
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

### 2. Неправильный формат переменных

**Правильно:**
```ini
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPassword=ваш_пароль
```

**Неправильно:**
```ini
Environment=Email:SmtpHost=smtp.hostinger.com  # Должно быть __ (двойное подчеркивание)
Environment="Email__SmtpPassword=ваш_пароль"    # Не нужны кавычки
```

### 3. Пароль содержит специальные символы

Если пароль содержит специальные символы (`$`, `&`, `!` и т.д.), экранируйте их или используйте кавычки:

```ini
Environment=Email__SmtpPassword='пароль$с$символами'
```

### 4. Сервис не видит переменные

Убедитесь, что переменные находятся в секции `[Service]`, а не в `[Unit]`:

```ini
[Service]
Type=notify
...
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPassword=пароль
...
```

## Тестирование отправки

Создайте тестовую страницу или используйте форму на сайте:

1. Откройте страницу Contact или About
2. Заполните форму
3. Отправьте
4. Проверьте логи:
   ```bash
   sudo journalctl -u cloudcity -f
   ```
5. Проверьте почтовый ящик support@cloudcity.center

## Контакты для поддержки

Если проблема не решается:
1. Проверьте логи (см. выше)
2. Проверьте настройки в панели Hostinger
3. Убедитесь, что почтовый ящик support@cloudcity.center создан и активен

