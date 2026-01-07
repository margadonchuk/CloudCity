# 📋 Пошаговая настройка SMTP (Hostinger)

## Шаг 1: Проверка текущей конфигурации

На сервере выполните скрипт диагностики:

```bash
chmod +x check-smtp-config.sh
sudo ./check-smtp-config.sh
```

Скрипт покажет:
- ✅ Какие переменные окружения установлены
- ❌ Какие отсутствуют
- 🌐 Доступность SMTP сервера
- 📝 Последние логи

## Шаг 2: Настройка переменных окружения

Откройте файл сервиса:

```bash
sudo systemctl edit cloudcity --full
```

Убедитесь, что в секции `[Service]` есть:

```ini
[Service]
...
# Email настройки (Hostinger SMTP)
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPort=587
Environment=Email__UseSsl=true
Environment=Email__SmtpUsername=support@cloudcity.center
Environment=Email__SmtpPassword=ВАШ_РЕАЛЬНЫЙ_ПАРОЛЬ_ОТ_ПОЧТЫ
Environment=Email__RecipientEmail=support@cloudcity.center
Environment=Email__CheckCertificate=true
...
```

**Важно:**
- Используйте двойное подчеркивание `__` (не `_` и не `:`)
- Пароль должен быть правильным (проверьте в панели Hostinger)
- Email должен быть полным: `support@cloudcity.center`

## Шаг 3: Применение изменений

```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

## Шаг 4: Проверка переменных

```bash
sudo systemctl show cloudcity | grep Email
```

Должны быть видны все переменные.

## Шаг 5: Тест подключения

```bash
# Тест порта 587
timeout 10 telnet smtp.hostinger.com 587

# Тест порта 465
timeout 10 telnet smtp.hostinger.com 465

# Тест через openssl
timeout 10 openssl s_client -connect smtp.hostinger.com:587 -starttls smtp
```

Если все таймаутят - Hostinger блокирует подключения.

## Шаг 6: Проверка в панели Hostinger

1. **Почтовый ящик:**
   - Зайдите в панель Hostinger
   - Проверьте, что `support@cloudcity.center` создан
   - Попробуйте войти через веб-интерфейс почты
   - Убедитесь, что пароль правильный

2. **SMTP настройки:**
   - Найдите настройки почты для домена
   - Включите "Разрешить внешние SMTP подключения"
   - Проверьте, нет ли блокировки по IP

3. **Логи Hostinger:**
   - Проверьте логи почтового сервера
   - Посмотрите, есть ли попытки подключения

## Шаг 7: Тестирование

1. Откройте сайт
2. Заполните форму на странице **Contact** или **About**
3. Отправьте форму
4. Проверьте логи:

```bash
sudo journalctl -u cloudcity -f
```

Должны быть видны:
```
Email configuration: Host=smtp.hostinger.com, Port=587, Username=support@cloudcity.center, SSL=True, PasswordSet=True
Connecting to SMTP server: smtp.hostinger.com:587
Using StartTLS (port 587)
Attempting to connect to smtp.hostinger.com:587...
SMTP connection established. IsConnected: True, IsAuthenticated: False
Authenticating as support@cloudcity.center...
SMTP authentication successful. IsAuthenticated: True
Sending email to support@cloudcity.center...
Email sent successfully
```

## Если порт 587 не работает

Попробуйте порт 465:

```bash
sudo systemctl edit cloudcity --full
```

Измените:
```ini
Environment=Email__SmtpPort=465
```

Затем:
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

## Если все еще таймаут

1. **Временно отключите проверку SSL** (только для диагностики):
   ```ini
   Environment=Email__CheckCertificate=false
   ```

2. **Проверьте файрвол:**
   ```bash
   sudo ufw status
   sudo ufw allow 587/tcp
   sudo ufw allow 465/tcp
   ```

3. **Свяжитесь с поддержкой Hostinger:**
   - Спросите про внешние SMTP подключения
   - Уточните, какой порт использовать
   - Попросите проверить логи на их стороне

## Альтернативные порты для тестирования

Если стандартные порты не работают, попробуйте:

**Порт 25 (без SSL):**
```ini
Environment=Email__SmtpPort=25
Environment=Email__UseSsl=false
```

⚠️ **ВНИМАНИЕ:** Порт 25 часто блокируется провайдерами.

---

**После каждого изменения не забывайте:**
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```


