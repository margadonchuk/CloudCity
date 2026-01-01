#!/bin/bash
# ПОЛНАЯ ДИАГНОСТИКА ПРОБЛЕМ

echo "=========================================="
echo "ПОЛНАЯ ДИАГНОСТИКА ПРОБЛЕМ"
echo "=========================================="
echo ""

# Переходим в правильную директорию
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

if [ -d "CloudCityCenter" ]; then
    cd CloudCityCenter
elif [ ! -f "CloudCityCenter.csproj" ]; then
    echo "Ошибка: запустите скрипт из корня репозитория"
    exit 1
fi

echo "=== 1. ПРОВЕРКА СЕРВИСА ==="
echo ""
if sudo systemctl is-active --quiet cloudcity 2>/dev/null; then
    echo "✓ Сервис cloudcity ЗАПУЩЕН"
else
    echo "✗ Сервис cloudcity НЕ ЗАПУЩЕН"
    echo ""
    echo "Последние ошибки:"
    sudo journalctl -u cloudcity -n 20 --no-pager 2>/dev/null | tail -15 || echo "Нет логов"
fi

echo ""
echo "=== 2. ПРОВЕРКА ПРИЛОЖЕНИЯ ==="
echo ""

# Проверяем процессы dotnet
if pgrep -f "CloudCityCenter.dll" > /dev/null; then
    echo "✓ Процесс приложения запущен"
    ps aux | grep "CloudCityCenter.dll" | grep -v grep
else
    echo "✗ Процесс приложения НЕ запущен"
fi

echo ""
# Проверяем порт 5000
if (netstat -tlnp 2>/dev/null | grep -q ":5000 ") || (ss -tlnp 2>/dev/null | grep -q ":5000 "); then
    echo "✓ Порт 5000 слушается"
    netstat -tlnp 2>/dev/null | grep ":5000 " || ss -tlnp 2>/dev/null | grep ":5000 "
else
    echo "✗ Порт 5000 НЕ слушается"
fi

echo ""
# Тестируем доступность
echo "Тестируем http://localhost:5000..."
if curl -s -f -m 5 http://localhost:5000 > /dev/null 2>&1; then
    echo "✓ Приложение отвечает на localhost:5000"
else
    echo "✗ Приложение НЕ отвечает на localhost:5000"
    echo "Пробуем получить ответ (первые 200 символов):"
    curl -s -m 5 http://localhost:5000 2>&1 | head -c 200
    echo ""
fi

echo ""
echo "=== 3. ПРОВЕРКА NGINX ==="
echo ""

if sudo systemctl is-active --quiet nginx 2>/dev/null; then
    echo "✓ Nginx запущен"
else
    echo "✗ Nginx НЕ запущен"
fi

echo ""
if sudo nginx -t 2>&1 | grep -q "successful"; then
    echo "✓ Конфигурация nginx правильная"
else
    echo "✗ Конфигурация nginx имеет ошибки:"
    sudo nginx -t 2>&1
fi

echo ""
# Проверяем что nginx проксирует на localhost:5000
NGINX_CONFIG="/etc/nginx/sites-enabled/cloudcitylife.com"
if [ -f "$NGINX_CONFIG" ]; then
    if grep -q "proxy_pass.*localhost:5000" "$NGINX_CONFIG"; then
        echo "✓ Nginx настроен проксировать на localhost:5000"
    else
        echo "✗ Nginx НЕ настроен проксировать на localhost:5000"
        echo "Проверьте конфигурацию: $NGINX_CONFIG"
    fi
else
    echo "⚠ Файл конфигурации nginx не найден: $NGINX_CONFIG"
    echo "Проверьте: ls -la /etc/nginx/sites-enabled/"
fi

echo ""
# Тестируем через nginx
echo "Тестируем https://cloudcitylife.com..."
if curl -s -f -m 5 https://cloudcitylife.com > /dev/null 2>&1; then
    echo "✓ Сайт доступен через домен"
else
    echo "✗ Сайт НЕ доступен через домен"
    echo "Пробуем получить ответ:"
    curl -s -m 5 -k https://cloudcitylife.com 2>&1 | head -c 200
    echo ""
fi

echo ""
echo "=== 4. ПРОВЕРКА ФАЙЛОВ И ПУТЕЙ ==="
echo ""

FULL_PATH=$(pwd)
DLL_PATH="$FULL_PATH/bin/Release/net8.0/CloudCityCenter.dll"

echo "Текущая директория: $FULL_PATH"
echo ""

if [ -f "$DLL_PATH" ]; then
    echo "✓ DLL найден: $DLL_PATH"
    ls -lh "$DLL_PATH"
else
    echo "✗ DLL НЕ найден: $DLL_PATH"
    echo "Нужно собрать приложение!"
fi

echo ""
DOTNET_PATH=$(which dotnet 2>/dev/null || echo "")
if [ -z "$DOTNET_PATH" ]; then
    DOTNET_PATH=$(find /home -name dotnet -type f 2>/dev/null | head -1)
fi

if [ -n "$DOTNET_PATH" ] && [ -f "$DOTNET_PATH" ]; then
    echo "✓ dotnet найден: $DOTNET_PATH"
    $DOTNET_PATH --version 2>/dev/null || echo "Не удалось получить версию"
else
    echo "✗ dotnet НЕ найден"
fi

echo ""
echo "=== 5. ПРОВЕРКА ПЕРЕМЕННЫХ ОКРУЖЕНИЯ ==="
echo ""

SERVICE_FILE="/etc/systemd/system/cloudcity.service"
if [ -f "$SERVICE_FILE" ]; then
    echo "Содержимое файла сервиса:"
    cat "$SERVICE_FILE"
else
    echo "✗ Файл сервиса не найден: $SERVICE_FILE"
fi

echo ""
echo "=== 6. ЛОГИ NGINX (последние ошибки) ==="
echo ""
if [ -f "/var/log/nginx/error.log" ]; then
    echo "Последние 10 ошибок nginx:"
    sudo tail -10 /var/log/nginx/error.log 2>/dev/null || echo "Не удалось прочитать логи"
else
    echo "Файл логов nginx не найден"
fi

echo ""
echo "=========================================="
echo "ДИАГНОСТИКА ЗАВЕРШЕНА"
echo "=========================================="
echo ""
echo "СЛЕДУЮЩИЕ ШАГИ:"
echo ""
echo "1. Если сервис не запущен, выполните:"
echo "   ./fix-service.sh"
echo ""
echo "2. Если приложение не отвечает, проверьте логи:"
echo "   sudo journalctl -u cloudcity -f"
echo ""
echo "3. Если nginx не работает, проверьте конфигурацию:"
echo "   sudo nginx -t"
echo "   sudo systemctl status nginx"
echo ""

