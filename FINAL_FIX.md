# Финальное исправление - что было сделано

## Исправленные проблемы:

1. ✅ **HSTS отключается за прокси** - добавлена проверка `UseReverseProxy` перед `UseHsts()`
2. ✅ **HTTPS редирект отключен за прокси** - это предотвращает ERR_TOO_MANY_REDIRECTS
3. ✅ **Правильная настройка ForwardedHeaders** для работы с nginx

## Текущая конфигурация:

В `appsettings.Production.json` установлено:
- `UseReverseProxy: true` - это отключает HTTPS редирект и HSTS
- Порт настроен на 5000

## Как запустить на сервере:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Обновите код
git pull

# Соберите
dotnet build --configuration Release

# Запустите (ВАЖНО: используйте --no-launch-profile!)
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"
export USE_REVERSE_PROXY=true
dotnet run --configuration Release --no-launch-profile
```

## Проверка что все работает:

1. **Приложение должно запуститься без ошибок**
2. **В логах должно быть:** `Now listening on: http://localhost:5000`
3. **Проверьте локально:**
   ```bash
   curl http://localhost:5000
   ```
   Должен вернуть HTML страницы

4. **Nginx должен проксировать на localhost:5000**

## Если все еще не работает:

1. **Проверьте логи приложения** - какие ошибки показывает?
2. **Проверьте nginx логи:** `sudo tail -f /var/log/nginx/error.log`
3. **Убедитесь что порт 5000 свободен:** `netstat -tlnp | grep 5000`
4. **Проверьте что nginx правильный:** `sudo nginx -t`

## Ключевые изменения в коде:

В `Program.cs` теперь:
- Проверяется `UseReverseProxy` из конфигурации
- Если `UseReverseProxy = true`, то НЕ используется:
  - `UseHttpsRedirection()` 
  - `UseHsts()`

Это предотвращает конфликты с nginx, который уже обрабатывает HTTPS.

