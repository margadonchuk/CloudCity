# CloudCity Shop

🌐 Интернет-магазин по аренде серверов, созданный с нуля на ASP.NET MVC и Entity Framework Core.

## ⚙️ Используемые технологии

- ASP.NET MVC (.NET 6/8)
- Entity Framework Core
- MS SQL Server
- Identity (регистрация и вход)
- Stripe / PayPal API (оплата)
- Bootstrap / Tailwind CSS (frontend)
- AdminLTE (панель администратора)

## 🔧 Возможности (будут реализованы)

- Каталог серверов по странам и характеристикам
- Личный кабинет клиента
- Панель администратора
- Онлайн-оплата аренды
- Автоматизация выдачи доступов

## 📦 Установка

1. Клонировать репозиторий:
```bash
git clone https://github.com/margadonchuk/CloudCity.git
```
2. Открыть проект в Visual Studio.
3. Убедиться, что настроена строка подключения к SQL Server.
4. Запустить `Update-Database` через Package Manager Console.
5. Запустить проект (F5).

### Использование .NET CLI

```bash
dotnet build
dotnet run
```
