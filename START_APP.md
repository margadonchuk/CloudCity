# Запуск приложения - пошаговая инструкция

## Проблема: "Connection refused" на порту 5000

Это означает что приложение **не запущено**. Нужно его запустить.

## Шаг 1: Проверьте что нет запущенных процессов

```bash
# Найдите процессы dotnet
ps aux | grep dotnet

# Если есть процессы - убейте их
pkill -f dotnet
# или
killall dotnet
```

## Шаг 2: Убедитесь что вы в правильной директории

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
pwd  # должно показать: /home/siteadmin/cloudcity/CloudCityCenter
```

## Шаг 3: Проверьте что код обновлен

```bash
git pull
```

## Шаг 4: Проверьте сборку

```bash
dotnet build --configuration Release
```

Должно быть: `Build succeeded. 0 Warning(s) 0 Error(s)`

## Шаг 5: Запустите приложение и СМОТРИТЕ ВЫВОД

```bash
# Установите переменные окружения
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true

# Запустите приложение (НЕ в фоне!)
dotnet run --configuration Release --no-launch-profile
```

**ВАЖНО:** 
- Запускайте **БЕЗ** символа `&` в конце
- **Смотрите весь вывод** - там должны быть сообщения об ошибках если что-то не так
- Должно появиться сообщение: `Now listening on: http://localhost:5000`

## Что искать в выводе:

### ✅ Хорошие признаки:
- `Now listening on: http://localhost:5000`
- `Hosting_environment: Production`
- Нет ошибок и исключений

### ❌ Плохие признаки:
- `Exception`, `Error`, `Failed`
- `Unable to start Kestrel`
- Любые красные сообщения об ошибках

## Если приложение запустилось:

Оставьте его работающим и в **ДРУГОМ терминале** проверьте:

```bash
curl http://localhost:5000
```

Должен вернуть HTML код страницы.

## Если приложение НЕ запускается:

**Скопируйте ВЕСЬ вывод** команды `dotnet run` и покажите:
1. Последние 50 строк вывода
2. Все сообщения об ошибках (Exception, Error)
3. На какой строке останавливается запуск

## Альтернатива: Запуск через systemd

Если хотите запускать как сервис:

```bash
# Создайте/обновите сервис
sudo nano /etc/systemd/system/cloudcity.service
```

Вставьте:

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
sudo systemctl start cloudcity
sudo systemctl status cloudcity
sudo journalctl -u cloudcity -f
```

