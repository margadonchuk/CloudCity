# Восстановление базы данных - решение проблемы с миграциями

## Проблема
Ошибка при миграции: `Column 'Id' in table 'AspNetRoles' is of a type that is invalid for use as a key column in an index.`

Это происходит потому, что миграции были созданы для SQLite (тип TEXT), а база данных на SQL Server.

## Решение

### Вариант 1: Отключить автоматические миграции (РЕКОМЕНДУЕТСЯ)

Я уже обновил `Program.cs` - теперь для SQL Server миграции не применяются автоматически. Это сохранит ваши данные.

**Что было сделано:**
- В `Program.cs` добавлена проверка типа базы данных
- Для SQL Server автоматические миграции отключены
- База данных будет работать с существующей схемой

**После обновления кода на сервере:**
```bash
cd /home/siteadmin/cloudcity
git pull
cd CloudCityCenter
dotnet build --configuration Release
sudo systemctl restart cloudcity
```

### Вариант 2: Восстановить базу данных из бэкапа (если есть)

Если у вас есть бэкап базы данных за последние дни:

```bash
# Восстановить из бэкапа через sqlcmd
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -Q "RESTORE DATABASE CloudCityDB FROM DISK = '/path/to/backup.bak' WITH REPLACE;"
```

### Вариант 3: Исправить схему вручную (если таблицы повреждены)

Если таблицы были повреждены миграцией, можно попробовать исправить:

```sql
-- Проверить текущий тип колонки
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetRoles' AND COLUMN_NAME = 'Id';

-- Если тип TEXT - изменить на nvarchar(450)
ALTER TABLE AspNetRoles ALTER COLUMN Id nvarchar(450) NOT NULL;
```

## После восстановления

1. **Проверьте подключение:**
```bash
sudo journalctl -u cloudcity -n 50 | grep -E "Database|Can Connect"
```

2. **Проверьте данные:**
Откройте админ-панель и убедитесь, что продукты отображаются.

3. **Обновите изображения (если нужно):**
Используйте `UPDATE_IMAGE_URLS.sql` для исправления путей к изображениям.

## Предотвращение проблемы в будущем

**НЕ запускайте `dotnet ef database update` на сервере с SQL Server, если миграции созданы для SQLite!**

Если нужно создать миграции для SQL Server:
1. На локальной машине с SQL Server создайте новую миграцию
2. Или используйте `EnsureCreated()` вместо миграций для SQL Server

