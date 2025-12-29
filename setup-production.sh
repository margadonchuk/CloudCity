#!/bin/bash
# ПОЛНАЯ НАСТРОЙКА ДЛЯ РАБОТЫ С ДОМЕНОМ
# Этот скрипт настроит все автоматически

set -e

echo "=========================================="
echo "НАСТРОЙКА CLOUDCITY ДЛЯ ДОМЕНА cloudcitylife.com"
echo "=========================================="
echo ""

# Переходим в правильную директорию
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Ищем CloudCityCenter
if [ -d "CloudCityCenter" ]; then
    cd CloudCityCenter
elif [ -f "CloudCityCenter.csproj" ]; then
    # Уже в CloudCityCenter
    true
else
    echo "Ошибка: запустите скрипт из корня репозитория (где есть CloudCityCenter/)"
    echo "Текущая директория: $(pwd)"
    exit 1
fi

echo "1. Проверяем dotnet..."
DOTNET_PATH=$(which dotnet || echo "/usr/bin/dotnet")
if [ ! -f "$DOTNET_PATH" ]; then
    DOTNET_PATH=$(find /home -name dotnet -type f 2>/dev/null | head -1)
    if [ -z "$DOTNET_PATH" ]; then
        echo "ОШИБКА: dotnet не найден!"
        exit 1
    fi
fi
echo "   ✓ dotnet: $DOTNET_PATH"

echo ""
echo "2. Собираем приложение..."
dotnet clean 2>/dev/null || true
dotnet restore
dotnet build --configuration Release

if [ ! -f "bin/Release/net8.0/CloudCityCenter.dll" ]; then
    echo "ОШИБКА: сборка не удалась!"
    exit 1
fi
echo "   ✓ Сборка успешна"

echo ""
echo "3. Создаем systemd сервис..."
FULL_PATH=$(pwd)
DLL_PATH="$FULL_PATH/bin/Release/net8.0/CloudCityCenter.dll"
CURRENT_USER=$(whoami)

sudo tee /etc/systemd/system/cloudcity.service > /dev/null <<EOF
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

echo "   ✓ Сервис создан"

echo ""
echo "4. Активируем сервис..."
sudo systemctl daemon-reload
sudo systemctl enable cloudcity
sudo systemctl restart cloudcity

echo "   Ожидаем запуск..."
sleep 7

echo ""
echo "5. Проверяем статус..."
if sudo systemctl is-active --quiet cloudcity; then
    echo "   ✓ Сервис ЗАПУЩЕН"
else
    echo "   ✗ ОШИБКА: сервис не запустился"
    echo "   Смотрите логи: sudo journalctl -u cloudcity -n 50"
    exit 1
fi

echo ""
echo "6. Проверяем что приложение слушает порт 5000..."
sleep 2
if (netstat -tlnp 2>/dev/null | grep -q ":5000 ") || (ss -tlnp 2>/dev/null | grep -q ":5000 "); then
    echo "   ✓ Порт 5000 слушается"
else
    echo "   ⚠ Порт 5000 не найден, но сервис работает"
fi

echo ""
echo "7. Проверяем доступность приложения..."
if curl -s -f http://localhost:5000 > /dev/null 2>&1; then
    echo "   ✓ Приложение отвечает на localhost:5000"
else
    echo "   ⚠ Приложение не отвечает, но сервис запущен"
    echo "   Проверьте логи: sudo journalctl -u cloudcity -f"
fi

echo ""
echo "8. Проверяем nginx..."
if sudo nginx -t 2>/dev/null; then
    echo "   ✓ Конфигурация nginx правильная"
    echo "   Перезагружаем nginx..."
    sudo systemctl reload nginx 2>/dev/null || sudo systemctl restart nginx
    echo "   ✓ Nginx перезагружен"
else
    echo "   ⚠ Проблемы с nginx, но приложение работает"
fi

echo ""
echo "=========================================="
echo "НАСТРОЙКА ЗАВЕРШЕНА!"
echo "=========================================="
echo ""
echo "Ваш сайт должен быть доступен:"
echo "  https://cloudcitylife.com"
echo ""
echo "Управление сервисом:"
echo "  sudo systemctl status cloudcity    # статус"
echo "  sudo systemctl restart cloudcity   # перезапуск"
echo "  sudo journalctl -u cloudcity -f    # логи"
echo ""
echo "Проверка работы:"
echo "  curl http://localhost:5000         # проверка приложения"
echo "  curl https://cloudcitylife.com     # проверка через домен"
echo ""
