# Инструкция по развертыванию на Ubuntu сервере

## Быстрый старт (без базы данных)

1. **Клонируйте репозиторий** (если еще не сделано):
   ```bash
   cd /home/siteadmin
   git clone https://github.com/margadonchuk/cloudcity.git
   cd cloudcity/CloudCityCenter
   ```

2. **Установите .NET SDK 8.0** (если не установлен):
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --version 8.0.0
   export PATH=$PATH:$HOME/.dotnet
   ```

3. **Соберите приложение**:
   ```bash
   dotnet restore
   dotnet build --configuration Release
   ```

4. **Запустите приложение** (для тестирования):
   ```bash
   export ASPNETCORE_ENVIRONMENT=Production
   export ASPNETCORE_URLS="http://localhost:5000"
   export USE_REVERSE_PROXY=true
   dotnet run --configuration Release --no-launch-profile
   ```
   
   **ВАЖНО:** Используйте флаг `--no-launch-profile`, чтобы не использовался Development профиль из launchSettings.json

## Настройка systemd сервиса

Создайте файл `/etc/systemd/system/cloudcity.service`:

```ini
[Unit]
Description=CloudCity ASP.NET Core App
After=network.target

[Service]
Type=notify
WorkingDirectory=/home/siteadmin/cloudcity/CloudCityCenter
ExecStart=/usr/bin/dotnet /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true
User=siteadmin

[Install]
WantedBy=multi-user.target
```

Затем:
```bash
sudo systemctl daemon-reload
sudo systemctl enable cloudcity
sudo systemctl start cloudcity
sudo systemctl status cloudcity
```

## Настройка nginx

Добавьте в `/etc/nginx/sites-available/cloudcitylife.com`:

```nginx
server {
    listen 80;
    server_name cloudcitylife.com www.cloudcitylife.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name cloudcitylife.com www.cloudcitylife.com;

    ssl_certificate /etc/letsencrypt/live/cloudcitylife.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/cloudcitylife.com/privkey.pem;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Forwarded-Host $server_name;
        proxy_cache_bypass $http_upgrade;
    }
}
```

Проверьте конфигурацию и перезапустите nginx:
```bash
sudo nginx -t
sudo systemctl reload nginx
```

## Проверка работы

После запуска проверьте логи:
```bash
sudo journalctl -u cloudcity -f
```

Сайт должен быть доступен по адресу: https://cloudcitylife.com

## Примечания

- Приложение работает с InMemory базой данных (без реальной БД)
- Данные не сохраняются между перезапусками
- Для постоянного хранения данных настройте подключение к базе данных в `appsettings.Production.json`

