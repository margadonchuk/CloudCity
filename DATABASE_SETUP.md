# Настройка подключения к базе данных на Ubuntu

## Проблема: данные не сохраняются

Если данные не сохраняются в базе данных, скорее всего приложение использует **InMemory базу данных** вместо реальной SQL Server базы данных.

## Решение

### 1. Проверьте переменную окружения на сервере Ubuntu

Выполните на сервере:

```bash
echo $ConnectionStrings__DefaultConnection
```

Если переменная не установлена или пустая, приложение будет использовать InMemory базу данных, и данные не будут сохраняться.

### 2. Установите переменную окружения

**Вариант A: В systemd сервисе (рекомендуется)**

Отредактируйте файл `/etc/systemd/system/cloudcity.service`:

```ini
[Unit]
Description=CloudCity ASP.NET Core App
After=network.target

[Service]
Type=notify
WorkingDirectory=/home/siteadmin/cloudcity/CloudCityCenter
ExecStart=/usr/bin/dotnet /home/siteadmin/cloudcity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"
User=siteadmin

[Install]
WantedBy=multi-user.target
```

**Важно:** Замените `ВАШ_ПАРОЛЬ` на реальный пароль от базы данных.

После редактирования:

```bash
sudo systemctl daemon-reload
sudo systemctl restart cloudcity
sudo systemctl status cloudcity
```

**Вариант B: В файле appsettings.Production.json**

Отредактируйте файл `CloudCityCenter/appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  ...
}
```

**Важно:** Замените `YOUR_PASSWORD_HERE` на реальный пароль.

### 3. Проверьте логи приложения

После перезапуска проверьте логи:

```bash
sudo journalctl -u cloudcity -f
```

Вы должны увидеть:
- ✅ `Database connection string found: Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=***`
- ✅ `Database Provider: Microsoft.EntityFrameworkCore.SqlServer`
- ✅ `Is InMemory: False`
- ✅ `Is Relational: True`
- ✅ `Can Connect: True`

Если видите:
- ⚠️ `WARNING: Using InMemory database!` - значит переменная окружения не установлена

### 4. Примените миграции базы данных

Если база данных пустая или нужно применить миграции:

```bash
cd /home/siteadmin/cloudcity/CloudCityCenter
export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"
dotnet ef database update
```

### 5. Заполните базу начальными данными (опционально)

```bash
dotnet run --project CloudCityCenter -- --seed
```

## Проверка работы

1. Создайте тестовый сервер через админ-панель
2. Проверьте, что он сохранился в базе данных
3. Перезапустите приложение
4. Убедитесь, что данные остались

## Диагностика

Приложение теперь логирует информацию о базе данных при запуске. Проверьте логи, чтобы убедиться, что используется правильная база данных, а не InMemory.

