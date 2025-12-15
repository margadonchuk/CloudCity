# Быстрое исправление проблемы ERR_TOO_MANY_REDIRECTS

## Что делать на сервере Ubuntu ПРЯМО СЕЙЧАС:

1. **Остановите приложение** (если запущено):
   ```bash
   sudo systemctl stop cloudcity
   # или если запущено вручную - Ctrl+C
   ```

2. **Обновите код**:
   ```bash
   cd /home/siteadmin/cloudcity
   git pull
   cd CloudCityCenter
   ```

3. **Пересоберите приложение**:
   ```bash
   dotnet build --configuration Release
   ```

4. **Запустите приложение с правильными настройками**:
   ```bash
   export ASPNETCORE_ENVIRONMENT=Production
   export ASPNETCORE_URLS="http://localhost:5000"
   export USE_REVERSE_PROXY=true
   dotnet run --configuration Release --no-launch-profile
   ```

5. **Или обновите systemd сервис** (если используете):
   
   Создайте/обновите файл `/etc/systemd/system/cloudcity.service`:
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
   sudo systemctl restart cloudcity
   sudo systemctl status cloudcity
   ```

6. **Проверьте логи** на ошибки:
   ```bash
   sudo journalctl -u cloudcity -f
   # или если запущено вручную - смотрите вывод в терминале
   ```

## Что было исправлено:

✅ Отключен HTTPS редирект когда приложение за nginx (предотвращает ERR_TOO_MANY_REDIRECTS)
✅ Отключен HSTS когда за прокси
✅ Добавлена поддержка переменной окружения USE_REVERSE_PROXY
✅ Настроен правильный порт 5000 в конфигурации
✅ Добавлен флаг --no-launch-profile чтобы не использовался Development профиль

## Проверка:

После запуска проверьте что:
- Приложение слушает на `http://localhost:5000`
- В логах показано `Hosting_environment: Production` (не Development!)
- Сайт открывается без ошибок редиректов

