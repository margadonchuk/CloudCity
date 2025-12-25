# Исправление ошибки "Unhandled exception" и таймаута systemd

## Проблема

По логам видно:
- Приложение запускается успешно
- Происходит "Unhandled exception" после старта
- Systemd убивает процесс из-за таймаута (Type=notify требует сигнала готовности)
- Счетчик перезапусков очень высокий (7393)

## Что было исправлено

1. **Добавлен глобальный обработчик необработанных исключений** - теперь все исключения логируются
2. **Добавлена поддержка systemd** - `AddSystemd()` и `UseSystemd()` для правильной работы с Type=notify
3. **Исправлена работа с сессией в _Layout.cshtml** - добавлена безопасная проверка доступности сессии
4. **Добавлен пакет Microsoft.Extensions.Hosting.Systemd** - для поддержки systemd интеграции

## Что нужно сделать на сервере

### 1. Обновите код и установите зависимости

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Обновите код
git pull

# Восстановите пакеты (добавится новый пакет для systemd)
dotnet restore

# Соберите приложение
dotnet build --configuration Release
```

### 2. Проверьте логи после перезапуска

```bash
# Остановите сервис
sudo systemctl stop cloudcity

# Перезапустите
sudo systemctl start cloudcity

# Следите за логами в реальном времени
sudo journalctl -u cloudcity -f
```

### 3. Если проблема сохраняется - измените Type=notify на Type=simple

Если `UseSystemd()` не решает проблему, измените тип сервиса:

```bash
sudo nano /etc/systemd/system/cloudcity.service
```

Измените:
```ini
[Service]
Type=simple  # Вместо Type=notify
```

Затем:
```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
```

### 4. Проверьте детальные логи исключений

После перезапуска проверьте логи на наличие детальной информации об исключении:

```bash
sudo journalctl -u cloudcity -n 200 --no-pager | grep -A 20 "Unhandled exception"
```

Или все логи:
```bash
sudo journalctl -u cloudcity -n 200 --no-pager
```

## Дополнительная диагностика

Если исключение все еще происходит, проверьте:

1. **Проблемы с базой данных:**
   ```bash
   # Проверьте подключение к БД
   echo $ConnectionStrings__DefaultConnection
   ```

2. **Проблемы с сессиями:**
   - Теперь сессии обрабатываются безопасно в _Layout.cshtml
   - Проверьте что `AddDistributedMemoryCache()` работает

3. **Проблемы с путями:**
   ```bash
   # Проверьте что все файлы на месте
   ls -la /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/
   ```

## Ожидаемый результат

После применения исправлений:
- Приложение должно запускаться без "Unhandled exception"
- Systemd должен получать сигнал готовности (или использовать Type=simple)
- Счетчик перезапусков должен перестать расти
- Логи должны показывать детальную информацию об ошибках (если они есть)

## Если проблема не решается

Соберите полную диагностическую информацию:

```bash
{
    echo "=== Статус сервиса ==="
    sudo systemctl status cloudcity --no-pager -l
    echo -e "\n=== Последние 200 строк логов ==="
    sudo journalctl -u cloudcity -n 200 --no-pager
    echo -e "\n=== Проверка порта ==="
    netstat -tlnp | grep 5000
    echo -e "\n=== Проверка процесса ==="
    ps aux | grep CloudCityCenter
} > full_diagnostics.txt
```

Отправьте файл `full_diagnostics.txt` для анализа.

