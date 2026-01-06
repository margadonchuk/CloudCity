# 📧 Настройка Email через Hostinger SMTP

## ✅ Что уже настроено

- ✅ EmailService с поддержкой Hostinger SMTP
- ✅ Правильные настройки порта (465) и SSL
- ✅ Формы на страницах About и Contact
- ✅ Чтение настроек из переменных окружения

## 🔐 Настройка на сервере (без пароля в Git)

### Вариант 1: Переменные окружения (Рекомендуется)

На сервере установите переменные окружения. Пароль **НЕ** будет в Git!

#### Для systemd service:

Отредактируйте файл сервиса (обычно `/etc/systemd/system/cloudcity.service`):

```ini
[Unit]
Description=CloudCity Center Application
After=network.target

[Service]
Type=notify
User=siteadmin
WorkingDirectory=/home/siteadmin/cloudcity/CloudCityCenter/CloudCity
ExecStart=/usr/bin/dotnet /home/siteadmin/cloudcity/CloudCityCenter/CloudCity/CloudCityCenter.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true

# Email настройки (Hostinger SMTP)
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPort=465
Environment=Email__UseSsl=true
Environment=Email__SmtpUsername=support@cloudcity.center
Environment=Email__SmtpPassword=ВАШ_ПАРОЛЬ_ОТ_ПОЧТЫ
Environment=Email__RecipientEmail=support@cloudcity.center

[Install]
WantedBy=multi-user.target
```

**Важно:** Замените `ВАШ_ПАРОЛЬ_ОТ_ПОЧТЫ` на реальный пароль от почтового ящика support@cloudcity.center

После редактирования:
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

#### Для запуска через скрипт:

Создайте или обновите файл `.env` в директории проекта (НЕ коммитьте в Git!):

```bash
# .env (добавьте в .gitignore!)
export Email__SmtpHost=smtp.hostinger.com
export Email__SmtpPort=465
export Email__UseSsl=true
export Email__SmtpUsername=support@cloudcity.center
export Email__SmtpPassword=ВАШ_ПАРОЛЬ_ОТ_ПОЧТЫ
export Email__RecipientEmail=support@cloudcity.center
```

И в скрипте запуска (`start-app.sh` или аналогичный):

```bash
#!/bin/bash
# Загружаем переменные окружения
source .env

# Остальной код...
dotnet run --configuration Release
```

### Вариант 2: Файл конфигурации на сервере (не в Git)

Создайте файл `appsettings.Local.json` на сервере (добавьте в `.gitignore`):

```json
{
  "Email": {
    "SmtpHost": "smtp.hostinger.com",
    "SmtpPort": 465,
    "UseSsl": true,
    "SmtpUsername": "support@cloudcity.center",
    "SmtpPassword": "ВАШ_ПАРОЛЬ_ОТ_ПОЧТЫ",
    "RecipientEmail": "support@cloudcity.center"
  }
}
```

И обновите `Program.cs` для чтения этого файла (если еще не добавлено):

```csharp
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Local.json", optional: true) // Локальный файл (не в Git)
    .AddEnvironmentVariables();
```

## 📋 Настройки Hostinger SMTP

Согласно документации Hostinger:

- **SMTP Host:** `smtp.hostinger.com`
- **Port:** `465`
- **SSL/TLS:** Включено (прямое SSL соединение)
- **Username:** `support@cloudcity.center` (ваш email)
- **Password:** Пароль от почтового ящика

## 🔍 Проверка работы

1. Заполните форму на странице **About** или **Contact**
2. Отправьте форму
3. Проверьте логи приложения:
   ```bash
   sudo journalctl -u cloudcity -f
   ```
4. Проверьте почтовый ящик **support@cloudcity.center**

## 🐛 Устранение неполадок

### Письма не отправляются

1. **Проверьте логи:**
   ```bash
   sudo journalctl -u cloudcity -n 50
   ```

2. **Проверьте переменные окружения:**
   ```bash
   sudo systemctl show cloudcity | grep Email
   ```

3. **Проверьте пароль:**
   - Убедитесь, что пароль правильный
   - Попробуйте подключиться к почте через почтовый клиент с теми же данными

4. **Проверьте порт:**
   - Hostinger использует порт **465** (не 587!)
   - Убедитесь, что порт не заблокирован файрволом

### Ошибка аутентификации

- Проверьте правильность email и пароля
- Убедитесь, что почтовый ящик создан в панели Hostinger
- Проверьте, что используете полный email (support@cloudcity.center), а не только имя

### Ошибка подключения

- Проверьте доступность `smtp.hostinger.com`
- Проверьте, что порт 465 открыт
- Убедитесь, что SSL правильно настроен

## 🔒 Безопасность

✅ **Правильно:**
- Хранить пароль в переменных окружения на сервере
- Использовать файл `appsettings.Local.json` (не в Git)
- Использовать секреты системы (systemd, Docker secrets и т.д.)

❌ **Неправильно:**
- Хранить пароль в `appsettings.Production.json` (будет в Git)
- Хранить пароль в коде
- Коммитить файлы с паролями в Git

## 📝 Пример .gitignore

Убедитесь, что в `.gitignore` есть:

```
# Локальные настройки
appsettings.Local.json
.env
*.env
```

## 🚀 Быстрая настройка

1. На сервере откройте файл сервиса:
   ```bash
   sudo nano /etc/systemd/system/cloudcity.service
   ```

2. Добавьте переменные окружения (см. выше)

3. Перезагрузите сервис:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart cloudcity
   ```

4. Проверьте логи:
   ```bash
   sudo journalctl -u cloudcity -f
   ```

Готово! Теперь письма будут отправляться через Hostinger SMTP, а пароль не будет в Git! 🎉


