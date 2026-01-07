# 🔧 Полная настройка SMTP для Hostinger

## Шаг 1: Проверка текущих настроек

На сервере выполните:

```bash
# Проверьте переменные окружения
sudo systemctl show cloudcity | grep Email

# Должны быть видны:
# Email__SmtpHost=smtp.hostinger.com
# Email__SmtpPort=587 (или 465)
# Email__SmtpUsername=support@cloudcity.center
# Email__SmtpPassword=*** (скрыто)
```

## Шаг 2: Тест подключения с сервера

```bash
# Тест порта 587
timeout 10 telnet smtp.hostinger.com 587

# Тест порта 465
timeout 10 telnet smtp.hostinger.com 465

# Тест через openssl (порт 587)
timeout 10 openssl s_client -connect smtp.hostinger.com:587 -starttls smtp

# Тест через openssl (порт 465)
timeout 10 openssl s_client -connect smtp.hostinger.com:465 -quiet
```

Если все таймаутят - Hostinger блокирует подключения с вашего IP.

## Шаг 3: Проверка настроек Hostinger

В панели управления Hostinger:

1. **Почтовый ящик:**
   - Убедитесь, что `support@cloudcity.center` создан
   - Проверьте пароль (попробуйте войти через веб-интерфейс)
   - Убедитесь, что почтовый ящик активен

2. **SMTP настройки:**
   - Найдите настройки "Внешние SMTP подключения"
   - Включите "Разрешить внешние SMTP подключения"
   - Проверьте, нет ли блокировки по IP адресу

3. **Логи Hostinger:**
   - Проверьте логи почтового сервера
   - Посмотрите, есть ли попытки подключения
   - Есть ли ошибки аутентификации

## Шаг 4: Настройка переменных окружения

Откройте файл сервиса:

```bash
sudo systemctl edit cloudcity --full
```

Убедитесь, что есть все переменные:

```ini
[Service]
...
# Email настройки (Hostinger SMTP)
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPort=587
Environment=Email__UseSsl=true
Environment=Email__SmtpUsername=support@cloudcity.center
Environment=Email__SmtpPassword=ВАШ_РЕАЛЬНЫЙ_ПАРОЛЬ
Environment=Email__RecipientEmail=support@cloudcity.center
Environment=Email__CheckCertificate=true
...
```

**Важно:**
- Используйте двойное подчеркивание `__` (не одинарное)
- Пароль должен быть правильным
- Email должен быть полным: `support@cloudcity.center`

## Шаг 5: Перезапуск

```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

## Шаг 6: Проверка логов

```bash
sudo journalctl -u cloudcity -f
```

Отправьте форму и смотрите логи. Должны быть видны:
- `Email configuration: Host=..., Port=..., Username=..., PasswordSet=True`
- `Connecting to SMTP server...`
- `Using StartTLS (port 587)` или `Using SSL on connect (port 465)`
- `SMTP connection established`
- `Authenticating as support@cloudcity.center...`
- `SMTP authentication successful`
- `Email sent successfully`

## Альтернативные варианты

### Вариант 1: Порт 465 с SSL

```ini
Environment=Email__SmtpPort=465
Environment=Email__UseSsl=true
```

### Вариант 2: Порт 587 с StartTLS

```ini
Environment=Email__SmtpPort=587
Environment=Email__UseSsl=true
```

### Вариант 3: Без проверки сертификата (только для диагностики)

```ini
Environment=Email__CheckCertificate=false
```

⚠️ **ВНИМАНИЕ:** Только для тестирования! После диагностики верните `true`.

## Если все еще не работает

1. **Свяжитесь с поддержкой Hostinger:**
   - Спросите про внешние SMTP подключения
   - Уточните, какой порт использовать
   - Спросите про блокировки по IP
   - Попросите проверить логи на их стороне

2. **Проверьте файрвол:**
   ```bash
   sudo ufw status
   sudo ufw allow 587/tcp
   sudo ufw allow 465/tcp
   ```

3. **Проверьте DNS:**
   ```bash
   nslookup smtp.hostinger.com
   ping smtp.hostinger.com
   ```

4. **Попробуйте другой SMTP сервис:**
   - Если Hostinger не позволяет внешние подключения
   - Используйте SendGrid, Mailgun или Web3Forms

---

**Помните:** После любых изменений обязательно перезапустите сервис!


