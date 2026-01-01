# Восстановление данных - полное руководство

## Ситуация
Код уже возвращен к рабочему состоянию. Теперь нужно проверить и восстановить данные в базе данных.

## Важно понимать
- Git хранит только код, а не данные базы данных
- Данные нужно восстанавливать из бэкапа базы данных
- Или данные уже есть в БД, просто нужно проверить

## Проверка данных в базе данных

### На сервере Ubuntu выполните:

```bash
# Проверить подключение к БД
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -d CloudCityDB -Q "SELECT COUNT(*) as ProductCount FROM Products; SELECT COUNT(*) as ServerCount FROM Servers;"
```

Если видите числа больше 0 - данные есть!

### Проверить конкретные таблицы:

```sql
-- Количество продуктов
SELECT COUNT(*) FROM Products;

-- Количество серверов
SELECT COUNT(*) FROM Servers;

-- Список продуктов
SELECT TOP 10 Id, Name, ImageUrl FROM Products;

-- Список серверов
SELECT TOP 10 Id, Name FROM Servers;
```

## Восстановление данных

### Вариант 1: Если есть бэкап базы данных

```bash
# Восстановить из бэкапа
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -Q "RESTORE DATABASE CloudCityDB FROM DISK = '/path/to/backup/CloudCityDB_backup.bak' WITH REPLACE;"
```

### Вариант 2: Если данных нет - заполнить начальными данными

Если база данных пустая, можно заполнить её начальными данными из SeedData:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=St123T123$%;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet run --project CloudCityCenter -- --seed
```

Это создаст базовые продукты (VPS, VPN, Hosting и т.д.).

### Вариант 3: Если нужно восстановить конкретные продукты

Если у вас были добавлены продукты через админ-панель, их нужно будет добавить заново, если нет бэкапа.

## Восстановление изображений

Изображения должны быть в папке `wwwroot/images/`. Если их нет:

1. Скопируйте с локальной машины на сервер через SCP/WinSCP
2. Или восстановите из git (если они в репозитории)

## Пошаговая инструкция восстановления

### Шаг 1: Проверить состояние базы данных

```bash
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -d CloudCityDB -Q "SELECT COUNT(*) as Products FROM Products; SELECT COUNT(*) as Servers FROM Servers; SELECT COUNT(*) as Orders FROM Orders;"
```

### Шаг 2: Если данных нет или мало - заполнить начальными данными

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=St123T123$%;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet run --project CloudCityCenter -- --seed
```

### Шаг 3: Проверить изображения

```bash
ls -la /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/*.png | wc -l
# Должно быть около 60 файлов
```

Если файлов нет - скопируйте с локальной машины.

### Шаг 4: Обновить пути к изображениям (если нужно)

Если продукты ссылаются на несуществующие изображения (vdi_*.png и т.д.), выполните UPDATE_IMAGE_URLS.sql:

```bash
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -d CloudCityDB -i /path/to/UPDATE_IMAGE_URLS.sql
```

### Шаг 5: Перезапустить приложение

```bash
sudo systemctl restart cloudcity
sudo systemctl status cloudcity
```

### Шаг 6: Проверить сайт

Откройте сайт в браузере и проверьте:
- Отображаются ли продукты
- Есть ли изображения
- Работает ли админ-панель

## Если ничего не помогает

1. Проверьте логи приложения:
```bash
sudo journalctl -u cloudcity -n 100
```

2. Проверьте логи базы данных на сервере SQL Server

3. Если есть возможность - восстановите базу данных из последнего рабочего бэкапа

