#!/bin/bash
# Скрипт для проверки миграции данных

echo "=== Проверка миграции данных в SQL Server ==="
echo ""

# Проверка строки подключения
echo "1. Проверка строки подключения..."
if [ -z "$ConnectionStrings__DefaultConnection" ]; then
    echo "   ⚠ Переменная окружения ConnectionStrings__DefaultConnection не установлена"
    echo "   Проверяю appsettings.Production.json..."
    
    if [ -f "CloudCityCenter/appsettings.Production.json" ]; then
        CONN_STR=$(grep -A 1 "DefaultConnection" CloudCityCenter/appsettings.Production.json | grep -o '"[^"]*"' | head -1 | tr -d '"')
        if [ ! -z "$CONN_STR" ]; then
            echo "   ✓ Строка подключения найдена в appsettings.Production.json"
            echo "   Первые 50 символов: ${CONN_STR:0:50}..."
        else
            echo "   ❌ Строка подключения не найдена в appsettings.Production.json"
        fi
    else
        echo "   ❌ Файл appsettings.Production.json не найден"
    fi
else
    echo "   ✓ Переменная окружения установлена"
    echo "   Первые 50 символов: ${ConnectionStrings__DefaultConnection:0:50}..."
fi

echo ""
echo "2. Запуск миграции с подробным выводом..."
echo ""

cd CloudCityCenter || exit 1

export ASPNETCORE_ENVIRONMENT=Production

# Запуск с подробным выводом
dotnet run -- --migrate-data 2>&1 | tee migration.log

echo ""
echo "=== Проверка завершена ==="
echo "Лог сохранен в migration.log"

