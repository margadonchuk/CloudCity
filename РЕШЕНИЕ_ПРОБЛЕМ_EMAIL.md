# 🔧 Решение проблем с отправкой Email

## Шаг 1: Проверка конфигурации

На сервере выполните:

```bash
# Сделайте скрипт исполняемым
chmod +x check-email-config.sh

# Запустите проверку
sudo ./check-email-config.sh
```

Скрипт покажет:
- ✅ Какие переменные окружения установлены
- ❌ Какие переменные отсутствуют
- 🌐 Доступность SMTP сервера
- 📝 Последние логи

## Шаг 2: Проверка логов

```bash
# Смотрите логи в реальном времени
sudo journalctl -u cloudcity -f

# Отправьте форму на сайте и смотрите, что появляется в логах
```

### Что должно быть в логах при успешной отправке:

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

## Шаг 3: Типичные проблемы и решения

### Проблема 1: "Email password not configured"

**В логах:**
```
Email password not configured. Set Email__SmtpPassword environment variable.
```

**Решение:**
1. Проверьте файл сервиса:
   ```bash
   sudo nano /etc/systemd/system/cloudcity.service
   ```

2. Убедитесь, что есть строка:
   ```ini
   Environment=Email__SmtpPassword=ваш_реальный_пароль
   ```

3. Перезагрузите сервис:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart cloudcity
   ```

4. Проверьте, что переменная установлена:
   ```bash
   sudo systemctl show cloudcity | grep Email__SmtpPassword
   ```

### Проблема 2: "Unable to connect to the remote server"

**В логах:**
```
Failed to send email. Error: Unable to connect to the remote server
```

**Решение:**

1. Проверьте доступность сервера:
   ```bash
   ping smtp.hostinger.com
   telnet smtp.hostinger.com 465
   ```

2. Проверьте файрвол:
   ```bash
   sudo ufw status
   # Если порт заблокирован, откройте:
   sudo ufw allow 465/tcp
   ```

3. Проверьте DNS:
   ```bash
   nslookup smtp.hostinger.com
   ```

### Проблема 3: "SMTP authentication failed"

**В логах:**
```
SMTP authentication failed
```

**Решение:**

1. Проверьте правильность email и пароля:
   - Email должен быть полным: `support@cloudcity.center`
   - Пароль должен быть правильным (проверьте в панели Hostinger)

2. Убедитесь, что почтовый ящик создан:
   - Зайдите в панель Hostinger
   - Проверьте, что почтовый ящик `support@cloudcity.center` существует
   - Попробуйте войти в веб-интерфейс почты с теми же данными

3. Проверьте, что пароль не содержит специальных символов, которые нужно экранировать

### Проблема 4: "SSL certificate validation failed"

**В логах:**
```
The SSL connection could not be established
```

**Решение (временное для диагностики):**

1. Добавьте в файл сервиса:
   ```ini
   Environment=Email__CheckCertificate=false
   ```

2. Перезапустите:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart cloudcity
   ```

⚠️ **ВНИМАНИЕ:** Это отключает проверку SSL сертификата. Используйте только для диагностики!

### Проблема 5: Переменные окружения не применяются

**Симптомы:** В логах видны значения по умолчанию, а не из переменных окружения

**Решение:**

1. Убедитесь, что переменные в секции `[Service]`:
   ```ini
   [Service]
   ...
   Environment=Email__SmtpHost=smtp.hostinger.com
   Environment=Email__SmtpPassword=пароль
   ...
   ```

2. Используйте двойное подчеркивание `__` (не одинарное `_`):
   - ✅ Правильно: `Email__SmtpHost`
   - ❌ Неправильно: `Email_SmtpHost` или `Email:SmtpHost`

3. Перезагрузите systemd:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart cloudcity
   ```

4. Проверьте:
   ```bash
   sudo systemctl show cloudcity | grep Email
   ```

## Шаг 4: Тестирование

1. Откройте сайт
2. Перейдите на страницу **Contact** или **About**
3. Заполните форму:
   - Имя: Тест
   - Email: ваш-email@example.com
   - Сообщение: Тестовое сообщение
4. Отправьте форму
5. Смотрите логи:
   ```bash
   sudo journalctl -u cloudcity -f
   ```
6. Проверьте почтовый ящик support@cloudcity.center

## Шаг 5: Дополнительная диагностика

Если ничего не помогает, создайте тестовый скрипт:

```bash
# test-smtp.sh
#!/bin/bash
echo "Testing SMTP connection..."
echo "QUIT" | openssl s_client -connect smtp.hostinger.com:465 -quiet
```

Или используйте telnet:
```bash
openssl s_client -connect smtp.hostinger.com:465
```

## Контакты

Если проблема не решается:
1. Соберите логи: `sudo journalctl -u cloudcity -n 200 > email-logs.txt`
2. Проверьте настройки в панели Hostinger
3. Убедитесь, что почтовый ящик активен и работает

---

**Помните:** После любых изменений в файле сервиса обязательно выполните:
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```


