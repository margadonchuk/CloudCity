# Диагностика проблем - пошаговая инструкция

## Шаг 1: Проверка компиляции

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
git pull
dotnet clean
dotnet restore
dotnet build --configuration Release
```

**Ожидаемый результат:** `Build succeeded. 0 Warning(s) 0 Error(s)`

Если есть ошибки компиляции - исправьте их перед продолжением.

## Шаг 2: Проверка файлов конфигурации

```bash
# Проверьте что файлы существуют
ls -la appsettings*.json

# Проверьте содержимое appsettings.Production.json
cat appsettings.Production.json
```

**Должно быть:**
```json
{
  "Logging": {...},
  "AllowedHosts": "*",
  "UseReverseProxy": true,
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

## Шаг 3: Попытка запуска с полным выводом

```bash
# Остановите все процессы dotnet
pkill -f dotnet || true

# Установите переменные окружения
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true

# Запустите и смотрите ВЕСЬ вывод
dotnet run --configuration Release --no-launch-profile 2>&1 | tee startup.log
```

**Что проверять в выводе:**
1. Есть ли сообщение `Now listening on: http://localhost:5000`?
2. Есть ли ошибки (Exception, Error, Fail)?
3. Что написано в строке `Hosting_environment:` - должно быть `Production`!

## Шаг 4: Проверка порта

В ДРУГОМ терминале (не останавливая приложение):

```bash
# Проверьте что порт 5000 слушается
netstat -tlnp | grep 5000
# или
ss -tlnp | grep 5000

# Попробуйте подключиться
curl -v http://localhost:5000
```

**Ожидаемый результат:** curl должен вернуть HTML код страницы (200 OK)

## Шаг 5: Проверка nginx

```bash
# Проверьте конфигурацию nginx
sudo nginx -t

# Проверьте что nginx запущен
sudo systemctl status nginx

# Проверьте логи nginx
sudo tail -50 /var/log/nginx/error.log

# Проверьте доступ к сайту через nginx
curl -v https://cloudcitylife.com
```

## Шаг 6: Проверка systemd сервиса (если используете)

```bash
# Проверьте статус
sudo systemctl status cloudcity

# Посмотрите логи
sudo journalctl -u cloudcity -n 100 --no-pager

# Проверьте что переменные окружения установлены
sudo systemctl show cloudcity | grep Environment
```

## Типичные проблемы и решения

### Проблема 1: "Connection refused" при curl localhost:5000

**Причина:** Приложение не запустилось или не слушает порт

**Решение:**
- Проверьте логи запуска (шаг 3)
- Убедитесь что порт 5000 не занят другим процессом: `sudo lsof -i :5000`
- Убейте процесс если нужно: `sudo kill -9 <PID>`

### Проблема 2: "ERR_TOO_MANY_REDIRECTS"

**Причина:** Используется HTTPS редирект когда за прокси

**Решение:**
- Убедитесь что `USE_REVERSE_PROXY=true` установлена
- Проверьте что `appsettings.Production.json` содержит `UseReverseProxy: true`
- Перезапустите приложение

### Проблема 3: Приложение запускается в Development режиме

**Причина:** Не установлена переменная окружения или используется launch profile

**Решение:**
- Убедитесь что используется флаг `--no-launch-profile`
- Проверьте переменную: `echo $ASPNETCORE_ENVIRONMENT` (должно быть Production)
- Проверьте что в логах: `Hosting_environment: Production`

### Проблема 4: "No such file or directory" при запуске

**Причина:** Неправильный путь или отсутствуют файлы

**Решение:**
- Убедитесь что вы в правильной директории: `pwd` должно показать `/home/siteadmin/cloudcity/CloudCityCenter`
- Проверьте что DLL существует: `ls -la bin/Release/net8.0/CloudCityCenter.dll`

### Проблема 5: Ошибки базы данных

**Причина:** Проблемы с InMemory базой или миграциями

**Решение:**
- Если используете InMemory БД, это нормально что данные не сохраняются
- Ошибки миграции логируются, но не должны останавливать запуск (есть try-catch)

## Сбор диагностической информации

Если ничего не помогло, соберите эту информацию:

```bash
# 1. Версия .NET
dotnet --version

# 2. Вывод запуска (сохраните в файл)
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true
dotnet run --configuration Release --no-launch-profile > startup.log 2>&1 &
sleep 5
cat startup.log

# 3. Статус процессов
ps aux | grep dotnet

# 4. Порт 5000
netstat -tlnp | grep 5000

# 5. Конфигурация nginx
sudo nginx -t
sudo cat /etc/nginx/sites-enabled/cloudcitylife.com

# 6. Логи nginx
sudo tail -50 /var/log/nginx/error.log
```

Пришлите эту информацию для дальнейшей диагностики.

