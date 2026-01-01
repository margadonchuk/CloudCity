#!/bin/bash
# Скрипт для проверки и обновления файлов на сервере

echo "=== Проверка файлов на сервере ==="

PROJECT_DIR="/home/siteadmin/CloudCity/CloudCityCenter"

# Проверка Hero файла
echo ""
echo "1. Проверка _Hero.cshtml:"
if grep -q "hero-section" "$PROJECT_DIR/Views/Home/_Hero.cshtml" 2>/dev/null; then
    echo "   ✓ Файл _Hero.cshtml содержит hero-section"
else
    echo "   ✗ Файл _Hero.cshtml НЕ содержит hero-section (нужно обновить)"
fi

# Проверка CSS
echo ""
echo "2. Проверка site.css:"
if grep -q "\.hero-section" "$PROJECT_DIR/wwwroot/css/site.css" 2>/dev/null; then
    echo "   ✓ Файл site.css содержит .hero-section"
    echo "   Количество упоминаний:"
    grep -c "hero-section" "$PROJECT_DIR/wwwroot/css/site.css" 2>/dev/null || echo "   0"
else
    echo "   ✗ Файл site.css НЕ содержит .hero-section (нужно обновить)"
fi

# Проверка Welcome файла
echo ""
echo "3. Проверка _Welcome.cshtml:"
if grep -q "welcome-section" "$PROJECT_DIR/Views/Home/_Welcome.cshtml" 2>/dev/null; then
    echo "   ✓ Файл _Welcome.cshtml содержит welcome-section"
else
    echo "   ✗ Файл _Welcome.cshtml НЕ содержит welcome-section (нужно обновить)"
fi

# Проверка JS
echo ""
echo "4. Проверка site.js:"
if grep -q "animate-on-scroll" "$PROJECT_DIR/wwwroot/js/site.js" 2>/dev/null; then
    echo "   ✓ Файл site.js содержит animate-on-scroll"
else
    echo "   ✗ Файл site.js НЕ содержит animate-on-scroll (нужно обновить)"
fi

# Проверка дат изменения файлов
echo ""
echo "5. Даты последнего изменения файлов:"
ls -lh "$PROJECT_DIR/Views/Home/_Hero.cshtml" 2>/dev/null | awk '{print "   _Hero.cshtml: " $6 " " $7 " " $8}'
ls -lh "$PROJECT_DIR/wwwroot/css/site.css" 2>/dev/null | awk '{print "   site.css: " $6 " " $7 " " $8}'
ls -lh "$PROJECT_DIR/wwwroot/js/site.js" 2>/dev/null | awk '{print "   site.js: " $6 " " $7 " " $8}'

echo ""
echo "=== Если файлы старые, выполните: ==="
echo "1. git pull origin main (если изменения в GitHub)"
echo "2. Или скопируйте файлы вручную"
echo "3. dotnet clean && dotnet build --configuration Release"
echo "4. sudo systemctl restart cloudcity"

