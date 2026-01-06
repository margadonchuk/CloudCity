# 🔧 Исправление миграции для SQL Server

## Проблема

Ошибка: `Column 'Id' in table 'AspNetRoles' is of a type that is invalid for use as a key column in an index`

Это происходит потому, что миграция `InitialCreate` была создана для SQLite (использует `TEXT`), а база данных - SQL Server (требует `NVARCHAR` или `VARCHAR`).

## Решение

### Вариант 1: Удалить старые миграции и создать новые (если база пустая)

**⚠️ ВНИМАНИЕ: Это удалит все данные! Используйте только если база пустая или вы сделали резервную копию!**

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Установите строку подключения
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ваш_пароль;TrustServerCertificate=True"

# Удалите все миграции
rm -rf Migrations/*

# Создайте новую начальную миграцию для SQL Server
dotnet ef migrations add InitialCreate --project CloudCityCenter

# Примените миграцию
dotnet ef database update --project CloudCityCenter

# Создайте миграцию для ContactMessages
dotnet ef migrations add AddContactMessages --project CloudCityCenter

# Примените миграцию
dotnet ef database update --project CloudCityCenter

# Перезапустите сервис
sudo systemctl restart cloudcity
```

### Вариант 2: Исправить существующую миграцию (если база уже используется)

**⚠️ Сложнее, но сохраняет данные**

1. Удалите проблемную миграцию `InitialCreate` из папки `Migrations/`
2. Создайте новую миграцию, которая будет правильно работать с SQL Server

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Установите строку подключения
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ваш_пароль;TrustServerCertificate=True"

# Удалите только InitialCreate миграцию (НЕ удаляйте другие!)
# Найдите файлы:
# - Migrations/20250911222054_InitialCreate.cs
# - Migrations/20250911222054_InitialCreate.Designer.cs
# И удалите их

# Создайте новую начальную миграцию
dotnet ef migrations add InitialCreateForSqlServer --project CloudCityCenter

# Проверьте, что миграция правильная (откройте файл и убедитесь, что использует NVARCHAR, а не TEXT)

# Примените миграцию
dotnet ef database update --project CloudCityCenter

# Создайте миграцию для ContactMessages
dotnet ef migrations add AddContactMessages --project CloudCityCenter

# Примените миграцию
dotnet ef database update --project CloudCityCenter

# Перезапустите сервис
sudo systemctl restart cloudcity
```

### Вариант 3: Применить только миграцию ContactMessages (если остальные таблицы уже существуют)

Если таблицы Identity (AspNetRoles, AspNetUsers и т.д.) уже существуют в БД, можно просто создать и применить миграцию только для ContactMessages:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter

# Установите строку подключения
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ваш_пароль;TrustServerCertificate=True"

# Удалите проблемную миграцию InitialCreate из истории (но НЕ из БД!)
# Откройте файл Migrations/ApplicationDbContextModelSnapshot.cs
# И удалите из него все упоминания о таблицах Identity

# Или проще - создайте миграцию, которая будет игнорировать существующие таблицы
dotnet ef migrations add AddContactMessagesOnly --project CloudCityCenter

# Откройте созданный файл миграции и убедитесь, что он создает только ContactMessages

# Примените миграцию
dotnet ef database update --project CloudCityCenter

# Перезапустите сервис
sudo systemctl restart cloudcity
```

## Проверка

После применения миграции:

1. Проверьте, что таблица создана:
   ```bash
   # Если есть доступ к SQL Server
   sqlcmd -S 10.151.10.8 -U sa -P ваш_пароль -d CloudCityDB -Q "SELECT COUNT(*) FROM ContactMessages"
   ```

2. Откройте `/Admin/Messages` в браузере - должна загрузиться страница без ошибок

3. Отправьте тестовую форму на странице Contact или About

4. Проверьте, что письмо появилось в `/Admin/Messages`

## Если ничего не помогает

Создайте таблицу вручную через SQL:

```sql
CREATE TABLE [ContactMessages] (
    [Id] INT NOT NULL IDENTITY(1,1),
    [Name] NVARCHAR(200) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL,
    [Phone] NVARCHAR(50) NULL,
    [Subject] NVARCHAR(200) NULL,
    [ServiceType] NVARCHAR(100) NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [SourcePage] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadAt] DATETIME2 NULL,
    CONSTRAINT [PK_ContactMessages] PRIMARY KEY ([Id])
);
```

Затем удалите миграцию `AddContactMessages` и создайте новую, которая будет пустой (таблица уже существует).

---

**Рекомендую использовать Вариант 1, если база пустая или вы сделали резервную копию!**

