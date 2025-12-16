#!/bin/bash
# Скрипт для настройки systemd сервиса CloudCity

echo "=== Настройка systemd сервиса CloudCity ==="

# Проверка пути к DLL
DLL_PATH="/home/siteadmin/CloudCity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll"
if [ ! -f "$DLL_PATH" ]; then
    echo "❌ ОШИБКА: DLL файл не найден по пути: $DLL_PATH"
    echo "Проверьте путь к файлу:"
    ls -la /home/siteadmin/CloudCity/CloudCityCenter/bin/Release/net8.0/
    exit 1
fi

echo "✓ DLL файл найден: $DLL_PATH"

# Запрос пароля БД
read -sp "Введите пароль от базы данных: " DB_PASSWORD
echo ""

# Создание systemd файла
SERVICE_FILE="/etc/systemd/system/cloudcity.service"

sudo tee "$SERVICE_FILE" > /dev/null <<EOF
[Unit]
Description=CloudCity ASP.NET Core App
After=network.target

[Service]
Type=notify
WorkingDirectory=/home/siteadmin/CloudCity/CloudCityCenter
ExecStart=/usr/bin/dotnet /home/siteadmin/CloudCity/CloudCityCenter/bin/Release/net8.0/CloudCityCenter.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true
Environment=ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=${DB_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true"
User=siteadmin

[Install]
WantedBy=multi-user.target
EOF

echo "✓ Systemd файл создан: $SERVICE_FILE"

# Перезагрузка systemd
echo "Перезагрузка systemd..."
sudo systemctl daemon-reload

# Остановка старого сервиса
echo "Остановка старого сервиса..."
sudo systemctl stop cloudcity 2>/dev/null || true

# Запуск сервиса
echo "Запуск сервиса..."
sudo systemctl start cloudcity

# Проверка статуса
echo ""
echo "=== Статус сервиса ==="
sudo systemctl status cloudcity --no-pager -l

echo ""
echo "=== Логи сервиса (последние 20 строк) ==="
sudo journalctl -u cloudcity -n 20 --no-pager

echo ""
echo "=== Готово! ==="
echo "Проверьте статус: sudo systemctl status cloudcity"
echo "Смотрите логи: sudo journalctl -u cloudcity -f"

