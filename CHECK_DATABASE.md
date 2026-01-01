# Проверка подключения к базе данных и отсутствующих изображений

## Проблема: 404 ошибки для изображений vdi_*.png

В логах видны ошибки 404 для изображений:
- `vdi_pro20_1.png`
- `vdi_start20_3.png`
- `vdi_standrt20_3.png`
- `vdi_pro20_3.png`

## Диагностика

### 1. Проверьте подключение к базе данных

Выполните на сервере Ubuntu:

```bash
sudo journalctl -u cloudcity -n 50 | grep -i "database\|connection\|inmemory"
```

Должны увидеть:
- ✅ `Database Provider: Microsoft.EntityFrameworkCore.SqlServer`
- ✅ `Is InMemory: False`
- ✅ `Can Connect: True`

Если видите `Is InMemory: True` - база данных не подключена!

### 2. Проверьте, какие продукты используют эти изображения

Подключитесь к базе данных SQL Server на Ubuntu:

```bash
# Установите sqlcmd если нужно
sudo apt-get install mssql-tools18 unixodbc-dev

# Подключитесь к базе
sqlcmd -S 10.151.10.8 -U sa -P "ВАШ_ПАРОЛЬ" -d CloudCityDB -Q "SELECT Name, ImageUrl FROM Products WHERE ImageUrl LIKE '%vdi%'"
```

Или через .NET:

```bash
cd /home/siteadmin/CloudCity/CloudCityCenter
dotnet run --project CloudCityCenter -- --check-images
```

### 3. Решение проблемы с изображениями

**Вариант A: Удалить продукты с отсутствующими изображениями**

Если эти продукты не нужны, удалите их из базы данных:

```sql
DELETE FROM Products WHERE ImageUrl LIKE '%vdi_pro20%' OR ImageUrl LIKE '%vdi_start20%' OR ImageUrl LIKE '%vdi_standrt20%';
```

**Вариант B: Загрузить изображения на сервер**

1. Скопируйте изображения в папку `wwwroot/images`:
```bash
cd /home/siteadmin/CloudCity/CloudCityCenter
# Загрузите файлы vdi_pro20_1.png, vdi_start20_3.png и т.д. в эту папку
sudo cp /путь/к/изображениям/*.png wwwroot/images/
```

2. Убедитесь, что файлы доступны:
```bash
ls -la wwwroot/images/vdi*.png
```

**Вариант C: Заменить пути к изображениям на существующие**

Обновите ImageUrl в базе данных на существующие изображения:

```sql
UPDATE Products 
SET ImageUrl = '/images/vps1.png' 
WHERE ImageUrl LIKE '%vdi_pro20%' OR ImageUrl LIKE '%vdi_start20%';
```

### 4. Проверьте, что данные сохраняются

Создайте тестовый продукт через админ-панель и проверьте:

```sql
SELECT TOP 5 * FROM Products ORDER BY Id DESC;
```

Если новые записи не появляются - проблема с сохранением данных (см. логи транзакций).

## Логи для проверки

```bash
# Логи подключения к БД
sudo journalctl -u cloudcity | grep -i "database\|connection"

# Логи транзакций
sudo journalctl -u cloudcity | grep -i "transaction\|commit\|rollback"

# Логи ошибок
sudo journalctl -u cloudcity | grep -i "error\|exception"
```

