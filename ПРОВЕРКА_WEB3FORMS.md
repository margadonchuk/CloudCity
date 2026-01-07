# ✅ Проверка Web3Forms

## Проблема

Форма показывает ошибку, хотя использует Web3Forms.

## Что проверить

### 1. Проверьте API ключ

На сервере откройте:
```bash
cat /home/siteadmin/cloudcity/CloudCityCenter/CloudCity/CloudCityCenter/wwwroot/js/web3forms-handler.js | grep WEB3FORMS_ACCESS_KEY
```

Должно быть:
```javascript
const WEB3FORMS_ACCESS_KEY = 'ваш-реальный-ключ';
```

**НЕ должно быть:**
```javascript
const WEB3FORMS_ACCESS_KEY = 'YOUR_WEB3FORMS_ACCESS_KEY';
```

### 2. Проверьте консоль браузера

1. Откройте сайт
2. Нажмите F12 (DevTools)
3. Перейдите на вкладку "Console"
4. Отправьте форму
5. Проверьте ошибки

**Если видите:**
```
❌ Web3Forms: API ключ не установлен!
```
→ Нужно установить API ключ

**Если видите:**
```
❌ Web3Forms error: {message: "Invalid access key"}
```
→ API ключ неправильный

**Если видите:**
```
❌ Failed to fetch
```
→ Проблема с сетью или CORS

### 3. Проверьте Network вкладку

1. В DevTools перейдите на "Network"
2. Отправьте форму
3. Найдите запрос к `api.web3forms.com`
4. Проверьте:
   - Статус ответа (должен быть 200)
   - Request payload (данные формы)
   - Response (ответ от API)

### 4. Получите новый API ключ

Если ключ не установлен или неправильный:

1. Откройте https://web3forms.com
2. Введите email: **support@cloudcity.center**
3. Нажмите "Get Your Access Key"
4. Скопируйте ключ из письма
5. Обновите файл на сервере

### 5. Обновите файл на сервере

```bash
nano /home/siteadmin/cloudcity/CloudCityCenter/CloudCity/CloudCityCenter/wwwroot/js/web3forms-handler.js
```

Найдите строку 9 и замените ключ.

**Важно:** После изменения файла перезапускать сервис не нужно - JavaScript загружается напрямую в браузере.

### 6. Очистите кеш браузера

После изменения файла:
- Нажмите Ctrl+Shift+R (жесткая перезагрузка)
- Или очистите кеш браузера

## Типичные ошибки

### Ошибка 1: "API ключ не установлен"
**Решение:** Получите ключ на https://web3forms.com и обновите файл

### Ошибка 2: "Invalid access key"
**Решение:** Проверьте, что ключ скопирован полностью, без лишних пробелов

### Ошибка 3: "Failed to fetch"
**Решение:** 
- Проверьте интернет соединение
- Проверьте, не блокирует ли файрвол api.web3forms.com
- Попробуйте другой браузер

### Ошибка 4: Форма не отправляется
**Решение:**
- Проверьте, что форма имеет класс `web3form`
- Проверьте консоль браузера на ошибки JavaScript
- Убедитесь, что файл `web3forms-handler.js` загружается

## Быстрая проверка

1. Откройте консоль браузера (F12)
2. Введите:
   ```javascript
   document.querySelectorAll('form.web3form').length
   ```
   Должно вернуть `2` (две формы: Contact и About)

3. Проверьте, что обработчик загружен:
   ```javascript
   typeof WEB3FORMS_ACCESS_KEY
   ```
   Должно вернуть `"string"` (не `"undefined"`)

---

**Проверьте консоль браузера - там будет видна точная причина ошибки!**


