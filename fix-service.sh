#!/bin/bash
# Скрипт для исправления проблем с systemd сервисом
# Этот скрипт диагностирует и исправляет проблемы с сервисом

echo "=========================================="
echo "ИСПРАВЛЕНИЕ ПРОБЛЕМ С СЕРВИСОМ"
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

FULL_PATH=$(pwd)
CURRENT_USER=$(whoami)

echo "1. Проверяем текущий статус..."
sudo systemctl status cloudcity --no-pager -l || true

echo ""
echo "2. Проверяем логи сервиса (последние 30 строк)..."
sudo journalctl -u cloudcity -n 30 --no-pager || true

echo ""
echo "3. Проверяем что dotnet существует..."
DOTNET_PATH=$(which dotnet 2>/dev/null || echo "")
if [ -z "$DOTNET_PATH" ]; then
    DOTNET_PATH=$(find /home -name dotnet -type f 2>/dev/null | head -1)
fi

if [ -z "$DOTNET_PATH" ] || [ ! -f "$DOTNET_PATH" ]; then
    echo "   ✗ ОШИБКА: dotnet не найден!"
    echo "   Установите .NET SDK 8.0"
    exit 1
fi
echo "   ✓ dotnet найден: $DOTNET_PATH"

echo ""
echo "4. Проверяем что DLL существует..."
DLL_PATH="$FULL_PATH/bin/Release/net8.0/CloudCityCenter.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "   ✗ DLL не найден. Собираем приложение..."
    if ! dotnet build --configuration Release; then
        echo "   ✗ ОШИБКА: сборка не удалась!"
        exit 1
    fi
    if [ ! -f "$DLL_PATH" ]; then
        echo "   ✗ ОШИБКА: DLL все еще не найден после сборки!"
        exit 1
    fi
fi
echo "   ✓ DLL найден: $DLL_PATH"

echo ""
echo "5. Проверяем файл сервиса..."
SERVICE_FILE="/etc/systemd/system/cloudcity.service"
if [ ! -f "$SERVICE_FILE" ]; then
    echo "   ✗ Файл сервиса не найден. Создаем..."
else
    echo "   ✓ Файл сервиса существует"
    echo "   Текущее содержимое:"
    cat "$SERVICE_FILE" | sed 's/^/   /'
fi

echo ""
echo "6. Обновляем файл сервиса с правильными путями..."

sudo tee "$SERVICE_FILE" > /dev/null <<EOF
[Unit]
Description=CloudCity ASP.NET Core App
After=network.target

[Service]
Type=notify
WorkingDirectory=$FULL_PATH
ExecStart=$DOTNET_PATH $DLL_PATH
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true
User=$CURRENT_USER

[Install]
WantedBy=multi-user.target
EOF

echo "   ✓ Файл сервиса обновлен"

echo ""
echo "7. Перезагружаем systemd и перезапускаем сервис..."
sudo systemctl daemon-reload
sudo systemctl enable cloudcity
sudo systemctl restart cloudcity

echo "   Ожидаем запуск (5 секунд)..."
sleep 5

echo ""
echo "8. Проверяем статус после перезапуска..."
if sudo systemctl is-active --quiet cloudcity; then
    echo "   ✓✓✓ СЕРВИС ЗАПУЩЕН УСПЕШНО!"
else
    echo "   ✗ Сервис все еще не запущен"
    echo ""
    echo "   Последние логи ошибок:"
    sudo journalctl -u cloudcity -n 50 --no-pager | tail -20
    echo ""
    echo "   Проверьте:"
    echo "   - Логи: sudo journalctl -u cloudcity -f"
    echo "   - Статус: sudo systemctl status cloudcity"
    exit 1
fi

echo ""
echo "9. Проверяем что приложение слушает порт..."
sleep 2
if (netstat -tlnp 2>/dev/null | grep -q ":5000 ") || (ss -tlnp 2>/dev/null | grep -q ":5000 "); then
    echo "   ✓ Приложение слушает порт 5000"
else
    echo "   ⚠ Порт 5000 не слушается (но сервис работает)"
fi

echo ""
echo "10. Тестируем доступность..."
if curl -s -f http://localhost:5000 > /dev/null 2>&1; then
    echo "   ✓ Приложение отвечает!"
else
    echo "   ⚠ Приложение не отвечает на запросы"
fi

echo ""
echo "=========================================="
echo "ГОТОВО!"
echo "=========================================="
echo ""
echo "Проверьте статус:"
echo "  sudo systemctl status cloudcity"
echo ""
echo "Смотрите логи:"
echo "  sudo journalctl -u cloudcity -f"
echo ""
echo "Проверьте сайт:"
echo "  curl https://cloudcitylife.com"

