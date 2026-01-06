#!/bin/bash
# Полная проверка конфигурации SMTP

echo "🔍 Полная диагностика SMTP конфигурации"
echo "========================================"
echo ""

# Цвета для вывода
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# 1. Проверка переменных окружения
echo "1️⃣ Проверка переменных окружения в systemd service:"
echo ""

SERVICE_FILE="/etc/systemd/system/cloudcity.service"
if [ ! -f "$SERVICE_FILE" ]; then
    echo -e "${RED}❌ Файл сервиса не найден: $SERVICE_FILE${NC}"
    exit 1
fi

VARS=("Email__SmtpHost" "Email__SmtpPort" "Email__SmtpUsername" "Email__SmtpPassword" "Email__RecipientEmail")
ALL_OK=true

for VAR in "${VARS[@]}"; do
    if grep -q "$VAR" "$SERVICE_FILE"; then
        VALUE=$(grep "$VAR" "$SERVICE_FILE" | sed 's/.*=//' | tr -d '"' | tr -d "'")
        if [ "$VAR" == "Email__SmtpPassword" ]; then
            if [ -z "$VALUE" ]; then
                echo -e "${RED}  ❌ $VAR: НЕ УСТАНОВЛЕН${NC}"
                ALL_OK=false
            else
                echo -e "${GREEN}  ✅ $VAR: установлен (скрыто)${NC}"
            fi
        else
            echo -e "${GREEN}  ✅ $VAR: $VALUE${NC}"
        fi
    else
        echo -e "${RED}  ❌ $VAR: НЕ НАЙДЕН${NC}"
        ALL_OK=false
    fi
done

echo ""
echo "2️⃣ Проверка активных переменных окружения:"
echo ""

SYSTEMCTL_VARS=$(sudo systemctl show cloudcity 2>/dev/null | grep Email || echo "")
if [ -z "$SYSTEMCTL_VARS" ]; then
    echo -e "${YELLOW}  ⚠️  Переменные окружения не найдены в активном сервисе${NC}"
    echo -e "${YELLOW}  💡 Выполните: sudo systemctl daemon-reload && sudo systemctl restart cloudcity${NC}"
    ALL_OK=false
else
    echo "$SYSTEMCTL_VARS" | while IFS= read -r line; do
        VAR_NAME=$(echo "$line" | cut -d'=' -f1)
        VAR_VALUE=$(echo "$line" | cut -d'=' -f2-)
        if [[ "$VAR_NAME" == *"Password"* ]]; then
            echo -e "${GREEN}  ✅ $VAR_NAME: [скрыто]${NC}"
        else
            echo -e "${GREEN}  ✅ $VAR_NAME: $VAR_VALUE${NC}"
        fi
    done
fi

echo ""
echo "3️⃣ Проверка доступности SMTP сервера:"
echo ""

SMTP_HOST="smtp.hostinger.com"
PORTS=(587 465)

for PORT in "${PORTS[@]}"; do
    echo -n "   Проверка порта $PORT... "
    if command -v nc &> /dev/null; then
        if timeout 5 nc -zv -w 3 $SMTP_HOST $PORT 2>&1 | grep -q "succeeded"; then
            echo -e "${GREEN}✅ доступен${NC}"
        else
            echo -e "${RED}❌ недоступен${NC}"
            ALL_OK=false
        fi
    else
        echo -e "${YELLOW}⚠️  netcat не установлен${NC}"
    fi
done

echo ""
echo "4️⃣ Тест SSL соединения:"
echo ""

if command -v openssl &> /dev/null; then
    echo "   Тест порта 587 (StartTLS)..."
    timeout 10 openssl s_client -connect $SMTP_HOST:587 -starttls smtp < /dev/null 2>&1 | head -3
    echo ""
    echo "   Тест порта 465 (SSL)..."
    timeout 10 openssl s_client -connect $SMTP_HOST:465 -quiet < /dev/null 2>&1 | head -3
else
    echo -e "${YELLOW}   ⚠️  openssl не установлен${NC}"
fi

echo ""
echo "5️⃣ Проверка DNS:"
echo ""

if command -v nslookup &> /dev/null; then
    nslookup $SMTP_HOST | grep -A 2 "Name:" || echo "   DNS запрос не удался"
else
    echo -e "${YELLOW}   ⚠️  nslookup не установлен${NC}"
fi

echo ""
echo "6️⃣ Последние логи Email (последние 30 строк):"
echo ""

sudo journalctl -u cloudcity -n 100 --no-pager 2>/dev/null | grep -i "email\|smtp" | tail -30 || echo "   Логи не найдены"

echo ""
echo "========================================"
if [ "$ALL_OK" = true ]; then
    echo -e "${GREEN}✅ Все основные настройки выглядят правильно${NC}"
    echo ""
    echo "💡 Если письма все еще не отправляются:"
    echo "   1. Проверьте настройки в панели Hostinger"
    echo "   2. Убедитесь, что разрешены внешние SMTP подключения"
    echo "   3. Проверьте логи: sudo journalctl -u cloudcity -f"
else
    echo -e "${RED}❌ Обнаружены проблемы в конфигурации${NC}"
    echo ""
    echo "💡 Исправьте проблемы выше и перезапустите сервис:"
    echo "   sudo systemctl daemon-reload"
    echo "   sudo systemctl restart cloudcity"
fi
echo ""

