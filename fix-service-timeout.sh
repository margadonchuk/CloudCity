#!/bin/bash
# Скрипт для исправления проблемы с timeout в systemd сервисе

echo "=== Исправление timeout в cloudcity.service ==="

# Остановим сервис принудительно
echo "1. Остановка сервиса..."
sudo systemctl stop cloudcity
sleep 2

# Проверим логи последней попытки запуска
echo ""
echo "=== Последние логи сервиса ==="
sudo journalctl -u cloudcity -n 50 --no-pager | tail -20

echo ""
echo "=== Обновление systemd файла с увеличенным timeout ==="

# Обновим systemd файл с увеличенным timeout
sudo tee /etc/systemd/system/cloudcity.service > /dev/null <<'EOF'
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
TimeoutStartSec=300
TimeoutStopSec=30
SyslogIdentifier=cloudcity
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=USE_REVERSE_PROXY=true
Environment=ConnectionStrings__DefaultConnection="Server=10.151.10.8;Database=CloudCityDB;User Id=sa;Password=ВАШ_ПАРОЛЬ_БД;TrustServerCertificate=True;MultipleActiveResultSets=true"
User=siteadmin

[Install]
WantedBy=multi-user.target
EOF

echo "✓ Systemd файл обновлен (добавлен TimeoutStartSec=300)"

# Перезагрузим systemd
echo ""
echo "2. Перезагрузка systemd..."
sudo systemctl daemon-reload

# Попробуем запустить
echo ""
echo "3. Запуск сервиса..."
sudo systemctl start cloudcity

# Подождем немного
sleep 5

# Проверим статус
echo ""
echo "=== Статус сервиса ==="
sudo systemctl status cloudcity --no-pager -l

echo ""
echo "=== Последние логи (20 строк) ==="
sudo journalctl -u cloudcity -n 20 --no-pager

echo ""
echo "=== Готово! ==="
echo "Если сервис не запустился, проверьте логи:"
echo "  sudo journalctl -u cloudcity -f"

