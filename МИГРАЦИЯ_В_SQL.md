# Миграция товаров и услуг в SQL Server

## Информация о серверах

- **SQL Server**: 10.151.10.8 (внутренний IP в сети CHR)
- **Site Server**: 10.151.10.10 (внутренний IP в сети CHR)
- **База данных**: CloudCityDB

## Способ 1: Использование SQL скрипта (рекомендуется)

1. Подключитесь к SQL Server на `10.151.10.8` через SQL Server Management Studio или другой клиент.

2. Убедитесь, что база данных `CloudCityDB` существует. Если нет, создайте её:
   ```sql
   CREATE DATABASE CloudCityDB;
   GO
   ```

3. Выполните миграции Entity Framework для создания структуры таблиц:
   ```bash
   cd CloudCityCenter
   dotnet ef database update
   ```
   Или примените миграции через приложение при первом запуске.

4. Выполните SQL скрипт `migrate_products_to_sql.sql`:
   - Откройте файл `migrate_products_to_sql.sql` в SQL Server Management Studio
   - Подключитесь к серверу `10.151.10.8`
   - Выберите базу данных `CloudCityDB`
   - Выполните скрипт (F5)

## Способ 2: Автоматическая миграция через приложение

1. Настройте строку подключения в `appsettings.Production.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"
     }
   }
   ```

2. Убедитесь, что файл `appsettings.Production.json` находится в `.gitignore` (уже добавлен).

3. Запустите миграцию данных:
   ```bash
   cd CloudCityCenter
   export ASPNETCORE_ENVIRONMENT=Production
   dotnet run -- --migrate-data
   ```

   Или используйте существующую команду:
   ```bash
   dotnet run -- --seed
   ```

## Что будет загружено

### Dedicated Servers (6 товаров):
- DS1-E3 (Germany) - €59/мес
- DS2-E5 (Netherlands) - €79/мес
- DS3-Ryzen (France) - €95/мес
- DS4-EPYC (Finland) - €129/мес
- DS5-Storage (USA) - €149/мес
- DS6-HighMem (Singapore) - €169/мес

### VPS (6 товаров):
- VPS1-1-20 - €10/мес
- VPS2-2-20 - €12/мес
- VPS3-4-40 - €19/мес
- VPS4-4-80 - €24/мес
- VPS5-4-80 - €28/мес
- VPS6-6-100 - €35/мес

### VPN Services (2 услуги):
- VPN for a network - €80/мес
- VPN for a device - €25/мес

### Hosting Services (1 услуга):
- Basic Hosting - €10/мес

### Website Services (1 услуга):
- Starter Website - €20/мес

**Всего: 16 товаров и услуг**

Каждый товар включает:
- Основную информацию (название, цена, конфигурация)
- Варианты оплаты (месячные/годовые)
- Характеристики (CPU, RAM, Storage, Network и т.д.)

## Проверка загрузки

После миграции проверьте данные:

```sql
USE CloudCityDB;
GO

-- Количество товаров
SELECT COUNT(*) as ProductsCount FROM Products;

-- Количество вариантов
SELECT COUNT(*) as VariantsCount FROM ProductVariants;

-- Количество характеристик
SELECT COUNT(*) as FeaturesCount FROM ProductFeatures;

-- Список всех товаров
SELECT Id, Name, Type, PricePerMonth, Location FROM Products;
```

## Важные замечания

1. **Безопасность**: Файлы `appsettings.*.json` уже добавлены в `.gitignore`, поэтому пароли не попадут в репозиторий.

2. **Повторная загрузка**: Скрипт проверяет наличие товаров перед загрузкой. Если товары уже есть, они не будут дублироваться.

3. **Миграции**: Убедитесь, что структура базы данных создана через Entity Framework миграции перед загрузкой данных.

4. **Подключение**: Приложение автоматически определит SQL Server по строке подключения (содержит `Server=`).

## Настройка на сервере сайта (10.151.10.10)

1. Скопируйте `appsettings.Production.json` на сервер (если еще не скопирован).

2. Установите переменную окружения или обновите файл с правильной строкой подключения:
   ```bash
   export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"
   ```

3. При первом запуске приложение автоматически применит миграции и загрузит данные (если их нет).

## Устранение проблем

### Ошибка подключения к SQL Server
- Проверьте, что SQL Server доступен с сервера сайта (10.151.10.10)
- Убедитесь, что SQL Server настроен на прослушивание TCP/IP
- Проверьте firewall правила

### Ошибка аутентификации
- Проверьте правильность логина и пароля в строке подключения
- Убедитесь, что пользователь имеет права на базу данных CloudCityDB

### Товары не загружаются
- Проверьте, что таблицы созданы (выполните миграции)
- Проверьте логи приложения на наличие ошибок

















