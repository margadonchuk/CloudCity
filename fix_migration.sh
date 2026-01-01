#!/bin/bash
# Скрипт для исправления миграции на SQL Server

set -e

echo "=== Исправление миграции для SQL Server ==="
echo ""

cd /home/siteadmin/cloudcity/CloudCityCenter || exit 1

# Проверка окружения
if [ -z "$ASPNETCORE_ENVIRONMENT" ]; then
    export ASPNETCORE_ENVIRONMENT=Production
    echo "✓ Установлен ASPNETCORE_ENVIRONMENT=Production"
fi

# Проверка строки подключения
if [ -z "$ConnectionStrings__DefaultConnection" ]; then
    echo "❌ ОШИБКА: ConnectionStrings__DefaultConnection не установлена"
    echo ""
    echo "Установите строку подключения:"
    echo 'export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"'
    exit 1
fi

echo "✓ Строка подключения установлена"
echo ""

# Проверка, что это SQL Server
if [[ ! "$ConnectionStrings__DefaultConnection" == *"Server="* ]]; then
    echo "❌ ОШИБКА: Строка подключения не содержит 'Server='. Это не SQL Server!"
    exit 1
fi

echo "Шаг 1: Создание резервной копии старых миграций..."
if [ -d "Migrations" ]; then
    BACKUP_DIR="Migrations_sqlite_backup_$(date +%Y%m%d_%H%M%S)"
    cp -r Migrations "$BACKUP_DIR"
    echo "✓ Резервная копия создана: $BACKUP_DIR"
else
    echo "⚠ Папка Migrations не найдена"
fi

echo ""
echo "Шаг 2: Удаление старых миграций..."
rm -rf Migrations
echo "✓ Старые миграции удалены"

echo ""
echo "Шаг 3: Создание новой миграции для SQL Server..."
dotnet ef migrations add InitialCreateForSqlServer --context ApplicationDbContext

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при создании миграции"
    echo "Восстанавливаем резервную копию..."
    if [ -d "$BACKUP_DIR" ]; then
        mv "$BACKUP_DIR" Migrations
    fi
    exit 1
fi

echo "✓ Миграция создана"

echo ""
echo "Шаг 4: Применение миграции к базе данных..."
dotnet ef database update --context ApplicationDbContext

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при применении миграции"
    exit 1
fi

echo "✓ Миграция применена"

echo ""
echo "Шаг 5: Загрузка данных..."
dotnet run -- --migrate-data

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при загрузке данных"
    exit 1
fi

echo ""
echo "✅ Все готово! Миграция исправлена и данные загружены."

