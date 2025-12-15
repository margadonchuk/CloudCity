#!/bin/bash
# Скрипт для запуска приложения на сервере

echo "Останавливаем приложение если запущено..."
sudo systemctl stop cloudcity 2>/dev/null || true

echo "Переходим в директорию проекта..."
cd /home/siteadmin/cloudcity/CloudCityCenter || exit 1

echo "Обновляем код из репозитория..."
git pull

echo "Собираем приложение..."
dotnet build --configuration Release

echo "Устанавливаем переменные окружения..."
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://localhost:5000"

echo "Запускаем приложение..."
echo "Приложение будет доступно на http://localhost:5000"
echo "Nginx должен проксировать запросы с cloudcitylife.com на localhost:5000"
echo ""
echo "Для остановки нажмите Ctrl+C"
echo ""

dotnet run --configuration Release --no-launch-profile

