#!/bin/bash
# Быстрая проверка и настройка файрвола для SMTP

echo "🔍 Проверка файрвола для SMTP"
echo ""

# Проверка UFW
if command -v ufw &> /dev/null; then
    echo "1️⃣ Проверка UFW:"
    UFW_STATUS=$(sudo ufw status | head -1)
    echo "   Статус: $UFW_STATUS"
    
    if echo "$UFW_STATUS" | grep -q "active"; then
        echo "   ⚠️  UFW активен"
        echo ""
        echo "   Проверка правил для портов 587 и 465:"
        sudo ufw status | grep -E "587|465" || echo "   ❌ Правила для портов 587/465 не найдены"
        echo ""
        read -p "   Разрешить исходящие подключения на порты 587 и 465? (y/n): " ALLOW_PORTS
        if [ "$ALLOW_PORTS" = "y" ] || [ "$ALLOW_PORTS" = "Y" ]; then
            echo "   Разрешаем порты..."
            sudo ufw allow out 587/tcp
            sudo ufw allow out 465/tcp
            sudo ufw reload
            echo "   ✅ Порты разрешены"
        fi
    else
        echo "   ✅ UFW не активен (порты не блокируются)"
    fi
else
    echo "   ⚠️  UFW не установлен"
fi

echo ""
echo "2️⃣ Проверка iptables:"
if command -v iptables &> /dev/null; then
    echo "   Проверка правил OUTPUT для портов 587 и 465:"
    sudo iptables -L OUTPUT -n -v 2>/dev/null | grep -E "587|465" || echo "   Правила не найдены или порты не блокируются"
    
    # Проверка политики по умолчанию
    DEFAULT_POLICY=$(sudo iptables -L OUTPUT -n | grep "Chain OUTPUT" | awk '{print $4}')
    echo "   Политика OUTPUT по умолчанию: $DEFAULT_POLICY"
    
    if [ "$DEFAULT_POLICY" = "DROP" ] || [ "$DEFAULT_POLICY" = "REJECT" ]; then
        echo "   ⚠️  Политика OUTPUT блокирует по умолчанию"
        read -p "   Добавить правила для портов 587 и 465? (y/n): " ADD_RULES
        if [ "$ADD_RULES" = "y" ] || [ "$ADD_RULES" = "Y" ]; then
            echo "   Добавляем правила..."
            sudo iptables -A OUTPUT -p tcp --dport 587 -j ACCEPT
            sudo iptables -A OUTPUT -p tcp --dport 465 -j ACCEPT
            echo "   ✅ Правила добавлены"
            echo "   💡 Сохраните правила: sudo iptables-save > /etc/iptables/rules.v4"
        fi
    else
        echo "   ✅ Политика OUTPUT разрешает по умолчанию"
    fi
else
    echo "   ⚠️  iptables не установлен"
fi

echo ""
echo "3️⃣ Тест подключения после изменений:"
echo "   Тестируем порт 587..."
timeout 5 nc -zv smtp.hostinger.com 587 2>&1 | head -1

echo ""
echo "4️⃣ Рекомендации:"
echo ""
if command -v ufw &> /dev/null && sudo ufw status | grep -q "active"; then
    echo "   ✅ UFW активен - проверьте правила выше"
else
    echo "   ℹ️  UFW не активен или не установлен"
fi

echo "   💡 Если подключение все еще не работает:"
echo "      1. Проверьте, не блокирует ли провайдер порты"
echo "      2. Свяжитесь с поддержкой VPS/хостинга"
echo "      3. Рассмотрите использование Web3Forms (не требует SMTP)"
echo ""


