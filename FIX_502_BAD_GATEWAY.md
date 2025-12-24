# Исправление ошибки 502 Bad Gateway

## Причины ошибки 502 Bad Gateway

Ошибка 502 возникает, когда nginx не может получить ответ от backend-приложения (ASP.NET Core). Основные причины:

1. **Таймауты nginx** - приложение отвечает слишком долго
2. **Падение backend-приложения** - приложение крашится или зависает
3. **Проблемы с подключением к БД** - долгие запросы к базе данных
4. **Недостаток ресурсов** - нехватка памяти или CPU
5. **Приложение не запущено** - процесс упал и не перезапустился

## Диагностика проблемы

### 1. Проверьте статус приложения

```bash
# Проверьте что приложение запущено
sudo systemctl status cloudcity

# Или проверьте процесс вручную
ps aux | grep dotnet

# Проверьте что приложение отвечает локально
curl http://localhost:5000
```

### 2. Проверьте логи приложения

```bash
# Логи systemd сервиса
sudo journalctl -u cloudcity -n 100 --no-pager

# Или логи в реальном времени
sudo journalctl -u cloudcity -f
```

### 3. Проверьте логи nginx

```bash
# Ошибки nginx
sudo tail -f /var/log/nginx/error.log

# Доступы nginx
sudo tail -f /var/log/nginx/access.log
```

### 4. Проверьте подключение к базе данных

```bash
# Проверьте что БД доступна
# Если используется SQL Server на 10.151.10.8
ping 10.151.10.8

# Проверьте строку подключения
echo $ConnectionStrings__DefaultConnection
```

## Решения проблемы

### Решение 1: Увеличить таймауты nginx

Отредактируйте `/etc/nginx/sites-available/cloudcitylife.com`:

```nginx
server {
    listen 443 ssl http2;
    server_name cloudcitylife.com www.cloudcitylife.com;

    ssl_certificate /etc/letsencrypt/live/cloudcitylife.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/cloudcitylife.com/privkey.pem;

    # Увеличиваем таймауты
    proxy_connect_timeout 300s;
    proxy_send_timeout 300s;
    proxy_read_timeout 300s;
    send_timeout 300s;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        
        # Заголовки для reverse proxy
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $server_name;
        
        # WebSocket поддержка
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_cache_bypass $http_upgrade;

        # Буферизация (может помочь с медленными ответами)
        proxy_buffering on;
        proxy_buffer_size 4k;
        proxy_buffers 8 4k;
        proxy_busy_buffers_size 8k;
    }
}
```

Примените изменения:
```bash
sudo nginx -t
sudo systemctl reload nginx
```

### Решение 2: Настроить автоматический перезапуск приложения

Убедитесь, что в `/etc/systemd/system/cloudcity.service` настроен автоматический перезапуск:

```ini
[Unit]
Description=CloudCity ASP.NET Core App
After=network.target

[Service]
Type=notify
User=siteadmin
WorkingDirectory=/home/siteadmin/cloudcity/CloudCityCenter
ExecStart=/usr/bin/dotnet /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true

[Install]
WantedBy=multi-user.target
```

Важные параметры:
- `Restart=always` - всегда перезапускать при падении
- `RestartSec=10` - ждать 10 секунд перед перезапуском

Примените изменения:
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

### Решение 3: Оптимизировать запросы к базе данных

Если проблема в медленных запросах к БД, добавьте таймауты в строку подключения:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=10.151.10.8;Database=CloudCityDB;User Id=...;Password=...;Connection Timeout=30;Command Timeout=60;"
  }
}
```

Или через переменную окружения:
```bash
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=...;Password=...;Connection Timeout=30;Command Timeout=60;"
```

### Решение 4: Увеличить лимиты ресурсов

Если проблема в нехватке памяти, увеличьте лимиты в systemd:

```ini
[Service]
# Увеличить лимит памяти (например, до 2GB)
MemoryLimit=2G

# Увеличить лимит CPU
CPUQuota=200%
```

### Решение 5: Использовать health check endpoint

Health check endpoint уже создан в `CloudCityCenter/Controllers/HealthController.cs`.

**Проверка работоспособности:**
```bash
# Простая проверка
curl http://localhost:5000/health

# Детальная проверка (с проверкой БД)
curl http://localhost:5000/health/detailed
```

**Настройте nginx для мониторинга:**
```nginx
location /health {
    proxy_pass http://localhost:5000;
    access_log off;
    proxy_connect_timeout 5s;
    proxy_read_timeout 5s;
}
```

### Решение 6: Мониторинг и алерты

Настройте мониторинг для автоматического обнаружения проблем:

```bash
# Создайте скрипт для проверки
cat > /home/siteadmin/check-app.sh << 'EOF'
#!/bin/bash
if ! curl -f http://localhost:5000/health > /dev/null 2>&1; then
    echo "Application is down, restarting..."
    sudo systemctl restart cloudcity
    # Можно добавить отправку уведомления
fi
EOF

chmod +x /home/siteadmin/check-app.sh

# Добавьте в crontab (проверка каждые 5 минут)
(crontab -l 2>/dev/null; echo "*/5 * * * * /home/siteadmin/check-app.sh") | crontab -
```

## Быстрая диагностика

Выполните эту команду для быстрой проверки всех компонентов:

```bash
echo "=== Проверка приложения ==="
sudo systemctl status cloudcity --no-pager -l

echo -e "\n=== Проверка порта 5000 ==="
netstat -tlnp | grep 5000

echo -e "\n=== Проверка локального ответа ==="
curl -I http://localhost:5000 2>&1 | head -5

echo -e "\n=== Последние ошибки nginx ==="
sudo tail -5 /var/log/nginx/error.log

echo -e "\n=== Последние логи приложения ==="
sudo journalctl -u cloudcity -n 20 --no-pager
```

## Профилактика

1. **Регулярно проверяйте логи** - настройте ротацию логов
2. **Мониторинг ресурсов** - следите за использованием памяти и CPU
3. **Оптимизируйте запросы к БД** - используйте индексы, избегайте N+1 запросов
4. **Настройте алерты** - получайте уведомления при падении приложения
5. **Регулярные обновления** - обновляйте систему и зависимости

## Если ничего не помогает

1. **Перезапустите все сервисы:**
   ```bash
   sudo systemctl restart cloudcity
   sudo systemctl restart nginx
   ```

2. **Проверьте системные ресурсы:**
   ```bash
   free -h
   df -h
   top
   ```

3. **Проверьте сетевые подключения:**
   ```bash
   netstat -tlnp
   ss -tlnp
   ```

4. **Временно увеличьте логирование:**
   - Включите подробные логи в приложении
   - Проверьте логи на наличие ошибок или исключений

