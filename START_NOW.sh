#!/bin/bash
# ПРОСТОЙ ЗАПУСК - выполните это

echo "=== ПРОВЕРКА И ЗАПУСК ==="
echo ""

cd /home/siteadmin/cloudcity/CloudCityCenter

echo "1. Статус сервиса:"
sudo systemctl status cloudcity --no-pager | head -15
echo ""

echo "2. Последние логи:"
sudo journalctl -u cloudcity -n 20 --no-pager
echo ""

echo "3. Проверка порта 5000:"
netstat -tlnp 2>/dev/null | grep 5000 || ss -tlnp 2>/dev/null | grep 5000 || echo "Порт 5000 не слушается"
echo ""

echo "4. Тест приложения:"
curl -s -m 3 http://localhost:5000 > /dev/null 2>&1 && echo "✓ Приложение отвечает" || echo "✗ Приложение НЕ отвечает"
echo ""

echo "5. Если приложение не работает, запускаем вручную:"
echo ""
echo "Выполните эти команды:"
echo ""
echo "cd /home/siteadmin/cloudcity/CloudCityCenter"
echo "export ASPNETCORE_ENVIRONMENT=Production"
echo "export ASPNETCORE_URLS=http://localhost:5000"
echo "export USE_REVERSE_PROXY=true"
echo "dotnet bin/Release/net8.0/CloudCityCenter.dll"
echo ""

