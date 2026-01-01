# Настройка systemd сервиса для постоянной работы

## Проблема

Приложение работает только пока запущено в терминале. Нужно настроить его как системный сервис.

## Решение: Создать systemd сервис

### Шаг 1: Создайте файл сервиса

```bash
sudo nano /etc/systemd/system/cloudcity.service
```

### Шаг 2: Вставьте следующее содержимое:

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

**ВАЖНО:** 
- Проверьте путь к dotnet: `which dotnet` (может быть `/home/siteadmin/.dotnet/dotnet`)
- Проверьте путь к DLL: `ls -la /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll`
- Убедитесь что пользователь `siteadmin` существует: `id siteadmin`

### Шаг 3: Если dotnet не в /usr/bin/dotnet:

Найдите где dotnet:
```bash
which dotnet
```

Если это `/home/siteadmin/.dotnet/dotnet`, замените в файле сервиса:
```ini
ExecStart=/home/siteadmin/.dotnet/dotnet /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
```

### Шаг 4: Активируйте сервис

```bash
# Перезагрузите systemd
sudo systemctl daemon-reload

# Включите автозапуск
sudo systemctl enable cloudcity

# Запустите сервис
sudo systemctl start cloudcity

# Проверьте статус
sudo systemctl status cloudcity
```

### Шаг 5: Проверьте что все работает

```bash
# Статус сервиса
sudo systemctl status cloudcity

# Логи в реальном времени
sudo journalctl -u cloudcity -f

# Проверка порта
curl http://localhost:5000

# Проверка через nginx
curl https://cloudcitylife.com
```

## Управление сервисом

```bash
# Запустить
sudo systemctl start cloudcity

# Остановить
sudo systemctl stop cloudcity

# Перезапустить
sudo systemctl restart cloudcity

# Статус
sudo systemctl status cloudcity

# Логи
sudo journalctl -u cloudcity -n 100
sudo journalctl -u cloudcity -f
```

## Если сервис не запускается

Проверьте логи:
```bash
sudo journalctl -u cloudcity -n 50 --no-pager
```

Типичные проблемы:
1. **Неправильный путь к dotnet** - проверьте `which dotnet`
2. **Неправильный путь к DLL** - проверьте что файл существует
3. **Проблемы с правами** - проверьте что пользователь `siteadmin` имеет доступ
4. **Порт занят** - проверьте `sudo lsof -i :5000`

## Альтернатива: Запуск в фоне без systemd

Если не хотите использовать systemd:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true

# Запустите в фоне
nohup dotnet run --configuration Release --no-launch-profile > app.log 2>&1 &

# Проверьте процесс
ps aux | grep dotnet

# Проверьте логи
tail -f app.log

# Остановить (найдите PID и убейте)
ps aux | grep dotnet
kill <PID>
```

Но systemd лучше - автоматический перезапуск, логи, управление.

