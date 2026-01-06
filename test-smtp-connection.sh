#!/bin/bash
# Скрипт для тестирования SMTP подключения к Hostinger

echo "🔍 Тестирование SMTP подключения к Hostinger"
echo ""

SMTP_HOST="smtp.hostinger.com"
SMTP_PORT="465"
EMAIL_USER="support@cloudcity.center"

echo "1. Проверка доступности порта $SMTP_PORT..."
if command -v nc &> /dev/null; then
    if nc -zv -w 5 $SMTP_HOST $SMTP_PORT 2>&1 | grep -q "succeeded"; then
        echo "   ✅ Порт $SMTP_PORT доступен"
    else
        echo "   ❌ Порт $SMTP_PORT недоступен"
        exit 1
    fi
else
    echo "   ⚠️  netcat не установлен, пропускаем проверку порта"
fi

echo ""
echo "2. Проверка SSL соединения..."
if command -v openssl &> /dev/null; then
    echo "   Подключение к $SMTP_HOST:$SMTP_PORT..."
    timeout 10 openssl s_client -connect $SMTP_HOST:$SMTP_PORT -quiet < /dev/null 2>&1 | head -5
    if [ ${PIPESTATUS[0]} -eq 0 ]; then
        echo "   ✅ SSL соединение установлено"
    else
        echo "   ❌ Не удалось установить SSL соединение"
    fi
else
    echo "   ⚠️  openssl не установлен, пропускаем проверку SSL"
fi

echo ""
echo "3. Проверка через telnet (если доступен)..."
if command -v telnet &> /dev/null; then
    echo "   Попытка подключения (прервите через 5 секунд если зависнет)..."
    timeout 5 telnet $SMTP_HOST $SMTP_PORT 2>&1 | head -3 || echo "   (таймаут или ошибка - это нормально для SSL порта)"
else
    echo "   ⚠️  telnet не установлен"
fi

echo ""
echo "4. Проверка DNS..."
if command -v nslookup &> /dev/null; then
    nslookup $SMTP_HOST | grep -A 2 "Name:"
else
    echo "   ⚠️  nslookup не установлен"
fi

echo ""
echo "📋 Рекомендации:"
echo ""
echo "Если все проверки прошли успешно, но письма не отправляются:"
echo "1. Проверьте логи приложения: sudo journalctl -u cloudcity -f"
echo "2. Проверьте настройки в панели Hostinger:"
echo "   - Разрешены ли внешние SMTP подключения?"
echo "   - Активен ли почтовый ящик $EMAIL_USER?"
echo "   - Правильный ли пароль?"
echo "3. Попробуйте подключиться через почтовый клиент (Thunderbird, Outlook)"
echo "4. Проверьте логи в панели Hostinger"
echo ""

