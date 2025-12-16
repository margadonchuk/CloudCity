#!/bin/bash
# Полный скрипт установки и миграции

set -e

echo "=== Установка EF Core Tools и миграция ==="
echo ""

cd /home/siteadmin/cloudcity/CloudCityCenter || exit 1

# Шаг 1: Установка EF Core Tools
echo "Шаг 1: Установка EF Core Tools..."
if ! command -v dotnet-ef &> /dev/null; then
    echo "Устанавливаю dotnet-ef глобально..."
    dotnet tool install --global dotnet-ef
    export PATH="$PATH:$HOME/.dotnet/tools"
else
    echo "✓ dotnet-ef уже установлен"
fi

# Проверка версии
dotnet ef --version
echo ""

# Шаг 2: Настройка окружения
echo "Шаг 2: Настройка окружения..."
export ASPNETCORE_ENVIRONMENT=Production

if [ -z "$ConnectionStrings__DefaultConnection" ]; then
    echo "❌ ОШИБКА: ConnectionStrings__DefaultConnection не установлена"
    echo ""
    echo "Установите строку подключения:"
    echo 'export ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ;TrustServerCertificate=True;MultipleActiveResultSets=true"'
    exit 1
fi

echo "✓ Окружение настроено"
echo ""

# Шаг 3: Удаление старых миграций
echo "Шаг 3: Удаление старых миграций..."
if [ -d "Migrations" ]; then
    BACKUP_DIR="Migrations_backup_$(date +%Y%m%d_%H%M%S)"
    cp -r Migrations "$BACKUP_DIR"
    echo "✓ Резервная копия создана: $BACKUP_DIR"
    rm -rf Migrations
    echo "✓ Старые миграции удалены"
else
    echo "⚠ Папка Migrations не найдена (возможно уже удалена)"
fi
echo ""

# Шаг 4: Создание новой миграции
echo "Шаг 4: Создание новой миграции для SQL Server..."
dotnet ef migrations add InitialCreateForSqlServer

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при создании миграции"
    exit 1
fi

echo "✓ Миграция создана"
echo ""

# Шаг 5: Применение миграции
echo "Шаг 5: Применение миграции к базе данных..."
echo "⚠ ВАЖНО: Если таблицы уже существуют, сначала удалите их через SQL скрипт drop_all_tables.sql"
read -p "Продолжить? (y/n) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Прервано пользователем"
    exit 1
fi

dotnet ef database update

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при применении миграции"
    echo "Возможно, нужно сначала удалить существующие таблицы через SQL скрипт"
    exit 1
fi

echo "✓ Миграция применена"
echo ""

# Шаг 6: Загрузка данных
echo "Шаг 6: Загрузка данных..."
dotnet run -- --migrate-data

if [ $? -ne 0 ]; then
    echo "❌ ОШИБКА при загрузке данных"
    exit 1
fi

echo ""
echo "✅ Все готово! Миграция завершена успешно."

