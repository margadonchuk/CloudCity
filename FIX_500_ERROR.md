# Исправление ошибки HTTP 500 на странице Windows Server

## Что было исправлено

1. **Добавлена обработка ошибок в ServersController**:
   - Весь метод `Index` обернут в try-catch
   - Все исключения логируются в консоль с полным stack trace
   - При ошибке возвращается пустая модель с инициализированными списками

2. **Добавлены безопасные проверки на null**:
   - Все списки планов проверяются на null перед добавлением в allPlans
   - Все списки фильтров инициализируются пустыми списками, если данные отсутствуют
   - Добавлены проверки на null в представлении для всех списков фильтров

3. **Инициализация пустых списков**:
   - Если товаров нет, возвращается модель с инициализированными пустыми списками
   - Это предотвращает NullReferenceException при обращении к спискам в представлении

## Что нужно проверить на сервере

### 1. Проверьте логи приложения

```bash
sudo journalctl -u cloudcity -n 100 --no-pager
```

Ищите строки с:
- `Error in ServersController.Index`
- `Stack trace`
- `Inner exception`

### 2. Проверьте что приложение запущено

```bash
sudo systemctl status cloudcity
```

### 3. Перезапустите приложение

```bash
sudo systemctl restart cloudcity
sudo journalctl -u cloudcity -f
```

### 4. Проверьте локально

```bash
curl http://localhost:5000/Servers
```

Если возвращается HTML (даже с ошибкой), значит приложение работает, но есть проблема в коде.
Если connection refused, значит приложение не запущено.

## Возможные причины ошибки 500

1. **Проблема с базой данных**:
   - Нет подключения к БД
   - Таблицы не существуют
   - Нет данных в таблице Products

2. **Проблема с Features или Variants**:
   - Товары есть, но нет Features или Variants
   - NullReferenceException при обращении к Features

3. **Проблема с локализацией**:
   - Отсутствуют ключи в .resx файлах

## Диагностика

Если ошибка сохраняется, соберите полную информацию:

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
    echo -e "\n=== Проверка локального ответа ==="
    curl -I http://localhost:5000/Servers 2>&1
} > error_500_diagnostics.txt
```

Отправьте файл `error_500_diagnostics.txt` для анализа.

## Быстрое исправление

Если проблема в коде, попробуйте:

1. **Пересобрать проект:**
   ```bash
   cd /home/siteadmin/cloudcity/CloudCityCenter
   dotnet clean
   dotnet build --configuration Release
   sudo systemctl restart cloudcity
   ```

2. **Проверить что все файлы на месте:**
   ```bash
   ls -la /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/
   ```

3. **Временно отключить фильтры** (если проблема в них):
   - Закомментируйте секцию фильтров в `Index.cshtml`
   - Перезапустите приложение
   - Если работает, проблема в фильтрах

