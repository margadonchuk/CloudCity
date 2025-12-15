# Быстрый старт - исправление проблем

## Проблема: ERR_TOO_MANY_REDIRECTS или сайт не открывается

### Решение (выполните на сервере Ubuntu):

```bash
# 1. Перейдите в директорию проекта
cd /home/siteadmin/cloudcity/CloudCityCenter

# 2. Обновите код
git pull

# 3. Соберите приложение
dotnet build --configuration Release

# 4. Остановите старый процесс (если запущен)
sudo systemctl stop cloudcity
# или найдите процесс: ps aux | grep dotnet
# и убейте: kill <PID>

# 5. Запустите с правильными настройками
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
dotnet run --configuration Release --no-launch-profile
```

### Или используйте скрипт:

```bash
chmod +x start-app.sh
./start-app.sh
```

## Проверка nginx конфигурации

Убедитесь что в `/etc/nginx/sites-enabled/cloudcitylife.com` есть:

```nginx
server {
    listen 443 ssl http2;
    server_name cloudcitylife.com www.cloudcitylife.com;

    ssl_certificate /etc/letsencrypt/live/cloudcitylife.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/cloudcitylife.com/privkey.pem;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $server_name;
    }
}
```

Перезагрузите nginx:
```bash
sudo nginx -t
sudo systemctl reload nginx
```

## Диагностика проблем

1. **Проверьте что приложение запущено:**
   ```bash
   curl http://localhost:5000
   ```
   Должен вернуть HTML код страницы

2. **Проверьте логи приложения:**
   Смотрите вывод в терминале где запущено приложение

3. **Проверьте логи nginx:**
   ```bash
   sudo tail -f /var/log/nginx/error.log
   ```

4. **Проверьте что порт 5000 слушается:**
   ```bash
   netstat -tlnp | grep 5000
   ```

## Что изменено в коде:

✅ В Production режиме полностью отключены HTTPS редирект и HSTS  
✅ Правильно настроены ForwardedHeaders для работы с nginx  
✅ Упрощена логика - нет сложных проверок переменных окружения

