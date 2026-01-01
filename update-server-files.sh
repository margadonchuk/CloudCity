#!/bin/bash
# Скрипт для обновления файлов на сервере (выполнять на сервере)

echo "=== Обновление файлов на сервере ==="

PROJECT_DIR="/home/siteadmin/CloudCity/CloudCityCenter"
BACKUP_DIR="/home/siteadmin/backup_$(date +%Y%m%d_%H%M%S)"

# Создаем резервную копию
echo "1. Создание резервной копии..."
mkdir -p "$BACKUP_DIR"
cp -r "$PROJECT_DIR/Views/Home" "$BACKUP_DIR/Views_Home" 2>/dev/null
cp "$PROJECT_DIR/wwwroot/css/site.css" "$BACKUP_DIR/site.css" 2>/dev/null
cp "$PROJECT_DIR/wwwroot/js/site.js" "$BACKUP_DIR/site.js" 2>/dev/null
echo "   ✓ Резервная копия создана: $BACKUP_DIR"

# Инструкции для пользователя
echo ""
echo "=== ИНСТРУКЦИЯ ==="
echo "1. Скопируйте обновленные файлы на сервер одним из способов:"
echo ""
echo "   Вариант A: Через SCP (с локальной машины):"
echo "   scp CloudCityCenter/Views/Home/_Hero.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/Views/Home/_Welcome.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/Views/Home/_About.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/Views/Home/_Testimonials.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/Views/Home/_Services.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/Views/Home/_Partners.cshtml siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/Views/Home/"
echo "   scp CloudCityCenter/wwwroot/css/site.css siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/wwwroot/css/"
echo "   scp CloudCityCenter/wwwroot/js/site.js siteadmin@СЕРВЕР:/home/siteadmin/CloudCity/CloudCityCenter/wwwroot/js/"
echo ""
echo "   Вариант B: Через WinSCP или FileZilla (GUI)"
echo "   Подключитесь к серверу и скопируйте файлы в соответствующие папки"
echo ""
echo "2. После копирования файлов выполните на сервере:"
echo "   cd /home/siteadmin/CloudCity/CloudCityCenter"
echo "   dotnet clean"
echo "   dotnet build --configuration Release"
echo "   sudo systemctl restart cloudcity"
echo ""
echo "3. Очистите кеш браузера (Ctrl+Shift+R)"

