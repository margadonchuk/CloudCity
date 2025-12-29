# 🔧 БЫСТРОЕ ИСПРАВЛЕНИЕ - САЙТ НЕ ДОСТУПЕН

## Шаг 1: Диагностика

```bash
cd /home/siteadmin/cloudcity
git pull
chmod +x diagnose.sh
./diagnose.sh
```

Этот скрипт покажет:
- Запущен ли сервис
- Работает ли приложение
- Работает ли nginx
- Есть ли ошибки

---

## Шаг 2: В зависимости от результата

### Если сервис не запущен:
```bash
chmod +x fix-service.sh
./fix-service.sh
```

### Если сервис запущен, но приложение не отвечает:

Проверьте логи:
```bash
sudo journalctl -u cloudcity -n 100 --no-pager
```

Попробуйте запустить вручную:
```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true
dotnet bin/Release/net8.0/CloudCityCenter.dll
```

**Смотрите какие ошибки показывает!**

### Если nginx не работает:

```bash
sudo nginx -t
sudo systemctl status nginx
sudo tail -50 /var/log/nginx/error.log
```

Проверьте конфигурацию nginx:
```bash
cat /etc/nginx/sites-enabled/cloudcitylife.com
```

Убедитесь что есть:
```nginx
location / {
    proxy_pass http://localhost:5000;
    ...
}
```

---

## Шаг 3: Если ничего не помогает

### Полная переустановка:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Останавливаем все
sudo systemctl stop cloudcity

# Собираем заново
dotnet clean
dotnet restore
dotnet build --configuration Release

# Запускаем вручную для проверки
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true
dotnet bin/Release/net8.0/CloudCityCenter.dll
```

Если вручную работает - настройте сервис заново:
```bash
./fix-service.sh
```

---

## Частые проблемы:

### 1. "Connection refused" на localhost:5000
- Приложение не запущено
- Запустите: `sudo systemctl start cloudcity`

### 2. "502 Bad Gateway"
- Приложение упало
- Проверьте логи: `sudo journalctl -u cloudcity -f`

### 3. "503 Service Unavailable"
- Nginx не может подключиться к приложению
- Проверьте что приложение слушает порт: `netstat -tlnp | grep 5000`

### 4. "ERR_TOO_MANY_REDIRECTS"
- Проблема с HTTPS редиректом
- Убедитесь что `USE_REVERSE_PROXY=true` установлен
- Проверьте `appsettings.Production.json` - должно быть `"UseReverseProxy": true`

### 5. Сайт не открывается вообще
- Проверьте DNS - указывает ли домен на ваш сервер?
- Проверьте файрвол: `sudo ufw status`
- Проверьте что порт 443 открыт

