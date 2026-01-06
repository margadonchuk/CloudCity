#!/bin/bash
# Скрипт для проверки конфигурации Email

echo "🔍 Проверка конфигурации Email (Hostinger SMTP)"
echo ""

# Проверка файла сервиса
SERVICE_FILE="/etc/systemd/system/cloudcity.service"

if [ ! -f "$SERVICE_FILE" ]; then
    echo "❌ Файл сервиса не найден: $SERVICE_FILE"
    exit 1
fi

echo "📋 Проверка переменных окружения в systemd service:"
echo ""

# Проверка каждой переменной
VARS=("Email__SmtpHost" "Email__SmtpPort" "Email__SmtpUsername" "Email__SmtpPassword" "Email__RecipientEmail")

for VAR in "${VARS[@]}"; do
    if grep -q "$VAR" "$SERVICE_FILE"; then
        VALUE=$(grep "$VAR" "$SERVICE_FILE" | sed 's/.*=//' | tr -d '"' | tr -d "'")
        if [ "$VAR" == "Email__SmtpPassword" ]; then
            if [ -z "$VALUE" ]; then
                echo "  ❌ $VAR: НЕ УСТАНОВЛЕН"
            else
                echo "  ✅ $VAR: установлен (скрыто)"
            fi
        else
            echo "  ✅ $VAR: $VALUE"
        fi
    else
        echo "  ❌ $VAR: НЕ НАЙДЕН"
    fi
done

echo ""
echo "📊 Проверка активных переменных окружения сервиса:"
echo ""

# Проверка через systemctl
SYSTEMCTL_VARS=$(sudo systemctl show cloudcity | grep Email)
if [ -z "$SYSTEMCTL_VARS" ]; then
    echo "  ⚠️  Переменные окружения не найдены в активном сервисе"
    echo "  💡 Выполните: sudo systemctl daemon-reload && sudo systemctl restart cloudcity"
else
    echo "$SYSTEMCTL_VARS" | while IFS= read -r line; do
        VAR_NAME=$(echo "$line" | cut -d'=' -f1)
        VAR_VALUE=$(echo "$line" | cut -d'=' -f2-)
        if [[ "$VAR_NAME" == *"Password"* ]]; then
            echo "  ✅ $VAR_NAME: [скрыто]"
        else
            echo "  ✅ $VAR_NAME: $VAR_VALUE"
        fi
    done
fi

echo ""
echo "🌐 Проверка доступности SMTP сервера:"
echo ""

# Проверка доступности порта
if command -v nc &> /dev/null; then
    if nc -zv smtp.hostinger.com 465 2>&1 | grep -q "succeeded"; then
        echo "  ✅ smtp.hostinger.com:465 доступен"
    else
        echo "  ❌ smtp.hostinger.com:465 недоступен"
        echo "     Проверьте файрвол и сетевые настройки"
    fi
else
    echo "  ⚠️  netcat (nc) не установлен, пропускаем проверку порта"
fi

echo ""
echo "📝 Последние логи Email (последние 20 строк):"
echo ""

# Показываем последние логи
sudo journalctl -u cloudcity -n 50 --no-pager | grep -i "email\|smtp" | tail -20

echo ""
echo "💡 Для просмотра всех логов в реальном времени:"
echo "   sudo journalctl -u cloudcity -f"
echo ""

