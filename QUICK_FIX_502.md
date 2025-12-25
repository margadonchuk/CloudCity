# Быстрое исправление ошибки 502 Bad Gateway

## Шаги для диагностики и исправления

### 1. Проверьте статус приложения на сервере

Подключитесь к серверу и выполните:

```bash
# Проверьте статус сервиса
sudo systemctl status cloudcity

# Если сервис не запущен или упал, перезапустите его
sudo systemctl restart cloudcity

# Проверьте что приложение запустилось
sudo systemctl status cloudcity
```

### 2. Проверьте логи приложения

```bash
# Последние 50 строк логов
sudo journalctl -u cloudcity -n 50 --no-pager

# Логи в реальном времени
sudo journalctl -u cloudcity -f
```

**Ищите:**
- Ошибки подключения к базе данных
- Исключения (Exceptions)
- Ошибки компиляции
- Проблемы с сессиями

### 3. Проверьте что приложение отвечает локально

```bash
# Проверка health check
curl http://localhost:5000/health

# Детальная проверка (с БД)
curl http://localhost:5000/health/detailed

# Проверка главной страницы
curl -I http://localhost:5000
```

Если приложение не отвечает локально, проблема в самом приложении, а не в nginx.

### 4. Проверьте логи nginx

```bash
# Последние ошибки nginx
sudo tail -20 /var/log/nginx/error.log

# Ищите сообщения типа:
# - "upstream timed out"
# - "connection refused"
# - "no live upstreams"
```

### 5. Быстрое исправление (если приложение упало)

```bash
# Перезапустите приложение
sudo systemctl restart cloudcity

# Подождите 5 секунд и проверьте статус
sleep 5
sudo systemctl status cloudcity

# Перезагрузите nginx
sudo systemctl reload nginx
```

### 6. Если проблема повторяется

**Проверьте использование ресурсов:**

```bash
# Память
free -h

# Диск
df -h

# Процессы
top
```

**Проверьте подключение к базе данных:**

```bash
# Проверьте что БД доступна
ping 10.151.10.8

# Проверьте строку подключения
echo $ConnectionStrings__DefaultConnection
```

**Увеличьте таймауты nginx** (если приложение отвечает медленно):

Отредактируйте `/etc/nginx/sites-available/cloudcitylife.com`:

```nginx
location / {
    proxy_pass http://localhost:5000;
    proxy_connect_timeout 300s;
    proxy_send_timeout 300s;
    proxy_read_timeout 300s;
    send_timeout 300s;
    # ... остальные настройки
}
```

Затем:
```bash
sudo nginx -t
sudo systemctl reload nginx
```

### 7. Проверьте что изменения применены

После исправления кода в CartController, убедитесь что:

1. Код скомпилирован без ошибок
2. Приложение перезапущено
3. Изменения применены на сервере

```bash
# На сервере, в директории проекта
cd /home/siteadmin/cloudcity/CloudCityCenter
dotnet build

# Если сборка успешна, перезапустите сервис
sudo systemctl restart cloudcity
```

## Частые причины 502 после изменений в коде

1. **Синтаксические ошибки** - проверьте логи компиляции
2. **NullReferenceException** - исправлено в CartController
3. **Проблемы с сессиями** - проверьте что сессии настроены в Program.cs
4. **Ошибки подключения к БД** - проверьте строку подключения

## Если ничего не помогает

1. **Временно отключите nginx и проверьте приложение напрямую:**
   ```bash
   # Остановите nginx
   sudo systemctl stop nginx
   
   # Проверьте приложение на порту 5000
   curl http://localhost:5000/health
   
   # Если работает, проблема в nginx
   # Если не работает, проблема в приложении
   ```

2. **Проверьте firewall:**
   ```bash
   sudo ufw status
   ```

3. **Проверьте что порт 5000 не занят другим процессом:**
   ```bash
   sudo netstat -tlnp | grep 5000
   ```

## Контакты для помощи

Если проблема не решается, соберите следующую информацию:

```bash
# Соберите диагностическую информацию
{
    echo "=== Статус сервиса ==="
    sudo systemctl status cloudcity --no-pager -l
    echo -e "\n=== Последние логи приложения ==="
    sudo journalctl -u cloudcity -n 100 --no-pager
    echo -e "\n=== Ошибки nginx ==="
    sudo tail -50 /var/log/nginx/error.log
    echo -e "\n=== Использование ресурсов ==="
    free -h
    df -h
} > diagnostics.txt
```

Отправьте файл `diagnostics.txt` для анализа.

