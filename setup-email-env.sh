#!/bin/bash
# Скрипт для настройки переменных окружения для Email (Hostinger SMTP)
# Использование: ./setup-email-env.sh

echo "🔧 Настройка переменных окружения для Email (Hostinger SMTP)"
echo ""

# Проверка, что скрипт запущен от root или с sudo
if [ "$EUID" -ne 0 ]; then 
    echo "⚠️  Этот скрипт должен быть запущен с правами root или через sudo"
    exit 1
fi

# Путь к файлу сервиса
SERVICE_FILE="/etc/systemd/system/cloudcity.service"

if [ ! -f "$SERVICE_FILE" ]; then
    echo "❌ Файл сервиса не найден: $SERVICE_FILE"
    echo "   Создайте сервис сначала или укажите правильный путь"
    exit 1
fi

echo "📝 Введите настройки SMTP:"
echo ""

read -p "Email адрес (support@cloudcity.center): " EMAIL_USERNAME
EMAIL_USERNAME=${EMAIL_USERNAME:-support@cloudcity.center}

read -sp "Пароль от почты: " EMAIL_PASSWORD
echo ""

if [ -z "$EMAIL_PASSWORD" ]; then
    echo "❌ Пароль не может быть пустым!"
    exit 1
fi

echo ""
echo "📋 Настройки:"
echo "   Host: smtp.hostinger.com"
echo "   Port: 465"
echo "   Username: $EMAIL_USERNAME"
echo "   Password: [скрыто]"
echo ""

read -p "Применить настройки? (y/n): " CONFIRM
if [ "$CONFIRM" != "y" ] && [ "$CONFIRM" != "Y" ]; then
    echo "Отменено"
    exit 0
fi

# Создаем резервную копию
cp "$SERVICE_FILE" "${SERVICE_FILE}.backup.$(date +%Y%m%d_%H%M%S)"
echo "✅ Создана резервная копия: ${SERVICE_FILE}.backup.*"

# Проверяем, есть ли уже настройки Email
if grep -q "Email__SmtpHost" "$SERVICE_FILE"; then
    echo "⚠️  Настройки Email уже существуют. Обновляем..."
    # Удаляем старые настройки Email
    sed -i '/Email__/d' "$SERVICE_FILE"
fi

# Добавляем настройки Email перед [Install]
sed -i '/\[Install\]/i\
# Email настройки (Hostinger SMTP)\
Environment=Email__SmtpHost=smtp.hostinger.com\
Environment=Email__SmtpPort=465\
Environment=Email__UseSsl=true\
Environment=Email__SmtpUsername='"$EMAIL_USERNAME"'\
Environment=Email__SmtpPassword='"$EMAIL_PASSWORD"'\
Environment=Email__RecipientEmail='"$EMAIL_USERNAME"'\
' "$SERVICE_FILE"

echo "✅ Настройки Email добавлены в файл сервиса"

# Перезагружаем systemd и перезапускаем сервис
echo ""
echo "🔄 Перезагружаем systemd..."
systemctl daemon-reload

echo "🔄 Перезапускаем сервис cloudcity..."
systemctl restart cloudcity

echo ""
echo "✅ Готово! Проверьте логи:"
echo "   sudo journalctl -u cloudcity -f"
echo ""
echo "📧 Теперь формы будут отправлять письма на $EMAIL_USERNAME"


