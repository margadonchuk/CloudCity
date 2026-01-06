# ✅ Финальная проверка SMTP (Hostinger)

## Подтвержденные настройки Hostinger

✅ **Сервер:** `smtp.hostinger.com`  
✅ **Порт:** `587` (TLS/STARTTLS) или `465` (SSL)  
✅ **Аутентификация:** Обязательна, логин = полный email, пароль = от ящика  
✅ **Внешние SMTP подключения:** Разрешены по умолчанию  
✅ **Ограничения по IP:** Нет  

## Критически важно: Двойное подчеркивание

В файле `/etc/systemd/system/cloudcity.service` **ОБЯЗАТЕЛЬНО** используйте **двойное подчеркивание** `__`:

```ini
[Service]
...
Environment=Email__SmtpHost=smtp.hostinger.com
Environment=Email__SmtpPort=587
Environment=Email__UseSsl=true
Environment=Email__SmtpUsername=support@cloudcity.center
Environment=Email__SmtpPassword=ваш_пароль
Environment=Email__RecipientEmail=support@cloudcity.center
...
```

**НЕ используйте:**
- ❌ `Email_SmtpPassword` (одно подчеркивание)
- ❌ `Email:SmtpPassword` (двоеточие)

**Используйте:**
- ✅ `Email__SmtpPassword` (двойное подчеркивание)

## Проверка конфигурации

### 1. Проверьте файл сервиса:

```bash
sudo systemctl cat cloudcity | grep Email
```

Должны быть видны все переменные с двойным подчеркиванием `__`.

### 2. Проверьте активные переменные:

```bash
sudo systemctl show cloudcity | grep Email
```

Должно быть:
```
Email__SmtpHost=smtp.hostinger.com
Email__SmtpPort=587
Email__UseSsl=true
Email__SmtpUsername=support@cloudcity.center
Email__SmtpPassword=*** (скрыто)
Email__RecipientEmail=support@cloudcity.center
```

### 3. Перезапустите сервис:

```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

### 4. Проверьте логи:

```bash
sudo journalctl -u cloudcity -f
```

Отправьте форму и смотрите. Должно быть:

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

## Если PasswordSet=False

Если в логах видно `PasswordSet=False`, значит пароль не читается:

1. **Проверьте двойное подчеркивание:**
   ```bash
   sudo systemctl cat cloudcity | grep SmtpPassword
   ```
   Должно быть: `Email__SmtpPassword=` (с двойным подчеркиванием)

2. **Проверьте, что пароль не пустой:**
   ```bash
   sudo systemctl show cloudcity | grep SmtpPassword
   ```
   Должно быть видно, что значение установлено (даже если скрыто)

3. **Если пароль содержит специальные символы:**
   Используйте одинарные кавычки:
   ```ini
   Environment=Email__SmtpPassword='пароль$с$символами'
   ```

## Тестирование

1. Откройте сайт
2. Заполните форму на странице **Contact** или **About**
3. Отправьте форму
4. Проверьте логи (см. выше)
5. Проверьте почтовый ящик **support@cloudcity.center**

## Если все еще не работает

1. **Проверьте пароль:**
   - Попробуйте войти в почтовый ящик через веб-интерфейс Hostinger
   - Убедитесь, что пароль правильный

2. **Попробуйте порт 465:**
   ```ini
   Environment=Email__SmtpPort=465
   ```
   Затем перезапустите:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart cloudcity
   ```

3. **Временно отключите проверку SSL** (только для диагностики):
   ```ini
   Environment=Email__CheckCertificate=false
   ```
   ⚠️ Только для тестирования! После диагностики верните `true`.

---

**Главное:** Убедитесь, что используется **двойное подчеркивание `__`** во всех переменных Email!

