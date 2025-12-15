# Проверка и настройка Nginx для домена

## Важно понимать архитектуру:

```
Интернет (HTTPS) → Nginx (443 порт) → Приложение (localhost:5000, HTTP)
     ↑                    ↑                    ↑
  Ваш домен          SSL сертификат      ASP.NET Core
cloudcitylife.com    обрабатывается      слушает только
                     здесь               локально
```

**Почему localhost:5000?**
- Nginx получает HTTPS запросы на домене
- Nginx обрабатывает SSL сертификат
- Nginx проксирует запросы на приложение по HTTP внутри сервера
- Это безопасно и стандартно для reverse proxy

## Проверка nginx конфигурации

Выполните на сервере:

```bash
# Проверьте что конфиг существует
ls -la /etc/nginx/sites-enabled/cloudcitylife.com

# Или проверьте все сайты
ls -la /etc/nginx/sites-enabled/
```

## Правильная конфигурация nginx

Файл `/etc/nginx/sites-available/cloudcitylife.com` или `/etc/nginx/sites-enabled/cloudcitylife.com`:

```nginx
# Редирект с HTTP на HTTPS
server {
    listen 80;
    server_name cloudcitylife.com www.cloudcitylife.com;
    return 301 https://$server_name$request_uri;
}

# Основной HTTPS сервер
server {
    listen 443 ssl http2;
    server_name cloudcitylife.com www.cloudcitylife.com;

    # SSL сертификаты (Let's Encrypt)
    ssl_certificate /etc/letsencrypt/live/cloudcitylife.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/cloudcitylife.com/privkey.pem;

    # Проксирование на приложение
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
    }
}
```

## Применить конфигурацию

```bash
# 1. Создайте или отредактируйте конфиг
sudo nano /etc/nginx/sites-available/cloudcitylife.com

# 2. Проверьте конфигурацию
sudo nginx -t

# 3. Если конфиг в sites-available, создайте симлинк
sudo ln -s /etc/nginx/sites-available/cloudcitylife.com /etc/nginx/sites-enabled/

# 4. Перезагрузите nginx
sudo systemctl reload nginx
```

## Проверка что все работает

```bash
# 1. Приложение слушает порт 5000
curl http://localhost:5000

# 2. Nginx проксирует на приложение
curl -k https://localhost

# 3. Домен работает
curl https://cloudcitylife.com

# 4. Логи nginx
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

## Если не работает

1. **Проверьте что приложение запущено:**
   ```bash
   sudo systemctl status cloudcity
   curl http://localhost:5000
   ```

2. **Проверьте что nginx запущен:**
   ```bash
   sudo systemctl status nginx
   ```

3. **Проверьте порты:**
   ```bash
   sudo netstat -tlnp | grep -E ':(80|443|5000) '
   ```

4. **Проверьте логи:**
   ```bash
   sudo journalctl -u cloudcity -n 50
   sudo tail -50 /var/log/nginx/error.log
   ```

