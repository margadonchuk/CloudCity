# CloudCity Center

🌐 Интернет-магазин по аренде серверов, созданный с нуля на ASP.NET MVC и Entity Framework Core.

An online shop for renting servers built from scratch with ASP.NET MVC and Entity Framework Core.

## ⚙️ Используемые технологии
**Technologies Used**

- ASP.NET MVC (.NET 6/8)
- Entity Framework Core
- MS SQL Server
- Identity (регистрация и вход)
- Stripe / PayPal API (оплата)
- Bootstrap / Tailwind CSS (frontend)
- AdminLTE (панель администратора)

## ✅ Предварительные требования
**Prerequisites**

- Установлен [.NET SDK 8.0](https://dotnet.microsoft.com/).  
  [.NET SDK 8.0](https://dotnet.microsoft.com/) installed.
- SQL Server LocalDB или другой совместимый сервер базы данных.  
  SQL Server LocalDB or another compatible database server.

## 🔧 Возможности (будут реализованы)
**Planned Features**

- Каталог серверов по странам и характеристикам
- Server catalog by country and specs
- Личный кабинет клиента
- Client dashboard
- Панель администратора
- Admin panel
- Онлайн-оплата аренды
- Online rental payments
- Автоматизация выдачи доступов
- Automated access provisioning

## 📦 Установка
**Installation**

1. Клонировать репозиторий:  
   Clone the repository:
   ```bash
   git clone https://github.com/yourusername/CloudCityCenter.git
   ```
2. Убедитесь, что выполнены все [предварительные требования](#-предварительные-требования).  
   Ensure all [prerequisites](#-предварительные-требования) are satisfied.
3. В корне репозитория выполните восстановление зависимостей:  
   In the repository root, restore dependencies:
   ```bash
   dotnet restore
   ```
4. Установите инструмент командной строки Entity Framework Core:  
   Install the Entity Framework Core CLI tool:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
   Этот шаг необходим перед выполнением `dotnet ef database update`.  
   This step is required before running `dotnet ef database update`.
5. Перейдите в папку `CloudCityCenter` и примените миграции:  
   Navigate to the `CloudCityCenter` folder and apply migrations:
   ```bash
   dotnet ef database update
   ```
6. После чистой установки база должна соответствовать модели. Проверьте это:
   After a clean install the database should match the model. Verify with:
   ```bash
   dotnet ef migrations list
   ```
   и запросом `HomeController.Index`.
   and by requesting `HomeController.Index`.
7. При необходимости заполните её примерными серверами и заказами:  
   (Optional) Seed the database with sample servers and orders:
   ```bash
   dotnet run --project CloudCityCenter -- seed
   ```
8. Запустите проект:  
   Run the project:
   ```bash
   dotnet run --project CloudCityCenter
   ```

По умолчанию для серверов используются изображения-заглушки с `via.placeholder.com`.  
By default placeholder images from `via.placeholder.com` are used for servers.  
Вы можете заменить их, загрузив собственные файлы в каталог `CloudCityCenter/wwwroot/images` и указав соответствующие URL в базе данных.  
You can replace them by uploading your own files to `CloudCityCenter/wwwroot/images` and updating the URLs in the database.

## 📚 Frontend библиотеки
**Frontend libraries**

Bootstrap и jQuery подключены через CDN в файле `_Layout.cshtml`. Локальная папка `wwwroot/lib` не используется.  
Bootstrap and jQuery are referenced from a CDN in `_Layout.cshtml`. The local `wwwroot/lib` folder is not used.

## 🧪 Запуск тестов
**Running tests**

Все модульные тесты находятся в проекте `CloudCityCenter.Tests`. Запустите их командой:  
All unit tests live in the `CloudCityCenter.Tests` project. Run them with:
```bash
dotnet test
```

## 🌱 Заполнение базы примерами
**Seeding the database**

После применения миграций можно заполнить базу данных тестовыми серверами и заказами с помощью класса `SeedData`. Выполните:
After applying migrations you can seed the database with test servers and orders using the `SeedData` class:
```bash
dotnet run --project CloudCityCenter -- seed
```
При этом будет создан тестовый пользователь `test@example.com` с паролем из переменной окружения `SEED_USER_PASSWORD` или случайно сгенерированным, а также несколько примерных заказов.
This will create the test user `test@example.com` with a password from the `SEED_USER_PASSWORD` environment variable (or a random one is generated and printed) along with a few example orders.

Если указать строку подключения к реальному SQL Server через переменную окружения `ConnectionStrings__DefaultConnection`, эта же команда заполнит именно его. После выполнения откройте `/Admin/Servers` (под аккаунтом администратора) и публичную страницу `/Servers`, чтобы убедиться, что товары созданы.
Supplying a real SQL Server connection string via the `ConnectionStrings__DefaultConnection` environment variable lets the same command populate that database. When it finishes, visit `/Admin/Servers` while signed in as an admin and the public `/Servers` page to verify the products were seeded.

## 🔗 Изменение строки подключения
**Changing the connection string**

Строка подключения `DefaultConnection` по умолчанию указывает на LocalDB.
The `DefaultConnection` connection string points to LocalDB by default.

Приложение автоматически выбирает провайдера: если в строке есть `Server=`, будет использован SQL Server, иначе — SQLite. Без строки подключения создаётся временная база в памяти.
The application automatically selects the provider: if `Server=` is present, SQL Server is used; otherwise SQLite. When no connection string is supplied, an in-memory database is created.

Укажите параметр `ConnectionStrings__DefaultConnection` в `CloudCityCenter/appsettings.Development.json` (этот файл не отслеживается системой контроля версий):
Set the `ConnectionStrings__DefaultConnection` value in `CloudCityCenter/appsettings.Development.json` (this file is ignored by source control):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;User Id=...;Password=..."
  }
}
```

Или задайте переменную окружения `ConnectionStrings__DefaultConnection`:
Or supply it via the `ConnectionStrings__DefaultConnection` environment variable:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost\SQLEXPRESS;Database=CloudCityDB;Trusted_Connection=True;"
```
⚠️ Используйте отдельную базу приложения (например, `CloudCityDB`), а не `master`.  
⚠️ Use a dedicated app database (for example, `CloudCityDB`), not `master`.

Или сохраните её в [секретах пользователя](https://learn.microsoft.com/aspnet/core/security/app-secrets) .NET:
Or store it using .NET [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

```bash
dotnet user-secrets init --project CloudCityCenter
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=...;User Id=...;Password=..."
```

После изменения строки подключения примените миграции и, при необходимости, заполните базу примерами:
After changing the connection string, apply migrations and optionally seed the database:
```bash
dotnet ef database update
dotnet run --project CloudCityCenter -- seed
```

## 🗄️ Миграции БД
**Database migrations**

- Создание миграции:
  Create a migration:
  ```bash
  dotnet ef migrations add <MigrationName>
  ```

- Применение миграций локально:
  Apply migrations locally:
  ```bash
  dotnet ef database update --project CloudCityCenter
  ```

- Применение миграций на сервере:
  Apply migrations on the server:
  ```bash
  export ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."
  dotnet ef database update --project CloudCityCenter
  sudo systemctl restart cloudcity.service
  ```
  Строка подключения передаётся через переменные окружения, затем перезапускается systemd‑сервис.
  The connection string is supplied via environment variables and the systemd service is restarted.

После применения миграций не забудьте обновить сервис.
Remember to update the service after applying migrations.

## 🖥️ Серверы (товары)
**Servers (Products)**

- Создание и применение миграций:
  Create and apply migrations:
  ```bash
  dotnet ef migrations add <MigrationName> --project CloudCityCenter
  dotnet ef database update --project CloudCityCenter
  ```
- Заполнение базы и администратора:
  Seed sample data and an admin account:
  ```bash
  dotnet run --project CloudCityCenter -- --seed
  dotnet run --project CloudCityCenter -- --seed-admin=admin@example.com
  ```
- URL панели администратора для серверов:
  Admin panel URL for servers:
  `/Admin/Servers`
- Поля модели `Server` и изображения:
  `Server` fields and image paths:
  Ключевые поля: `Name`, `Slug` (auto), `Description`, `Location`, `PricePerMonth`, `CPU`, `RamGb`, `StorageGb`, `ImageUrl`, `IsActive`, `DDoSTier`, `Stock`, `CreatedUtc`.
  Key fields: `Name`, `Slug` (auto), `Description`, `Location`, `PricePerMonth`, `CPU`, `RamGb`, `StorageGb`, `ImageUrl`, `IsActive`, `DDoSTier`, `Stock`, `CreatedUtc`.
  `ImageUrl` сохраняется как `/images/servers/<файл>`, изображения размещайте в `wwwroot/images/servers`.
  `ImageUrl` is stored as `/images/servers/<file>`; place images in `wwwroot/images/servers`.
