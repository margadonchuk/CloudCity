# Решение проблемы с отсутствующими изображениями

## Проблема
В базе данных есть все продукты, но изображения отсутствуют на сервере. В таблице видны пути к изображениям, которых нет:
- `/images/vdi_start_1.png`, `/images/vdi_standrt_*.png`, `/images/vdi_pro_*.png` (VDI продукты)
- `/images/hosting1.png`, `/images/hosting2.png`, `/images/hosting3.png` (Hosting продукты)
- `/images/nas1.png`, `/images/nas2.png`, `/images/nas3.png` (NAS продукты)
- `/images/Builder1.png`, `/images/Builder2.png`, `/images/Builder3.png` (Builder продукты)
- `/images/Developer*.png` (Developer продукты)

## Решения

### Вариант 1: Заменить пути в базе данных на существующие изображения (БЫСТРОЕ РЕШЕНИЕ)

Выполните SQL запросы на сервере Ubuntu для обновления путей:

```sql
-- Для VDI продуктов - заменить на hosting.png или webhosting.png
UPDATE Products 
SET ImageUrl = '/images/hosting.png' 
WHERE ImageUrl LIKE '%vdi%';

-- Для Hosting продуктов (hosting1, hosting2, hosting3) - заменить на hosting.png
UPDATE Products 
SET ImageUrl = '/images/hosting.png' 
WHERE ImageUrl LIKE '%hosting1%' OR ImageUrl LIKE '%hosting2%' OR ImageUrl LIKE '%hosting3%';

-- Для NAS продуктов - заменить на storage.png
UPDATE Products 
SET ImageUrl = '/images/storage.png' 
WHERE ImageUrl LIKE '%nas%';

-- Для Builder продуктов - заменить на webdev.png
UPDATE Products 
SET ImageUrl = '/images/webdev.png' 
WHERE ImageUrl LIKE '%Builder%';

-- Для Developer продуктов (Landing Pro, Site Pro, E-Commerce Pro) - заменить на webdev.png
UPDATE Products 
SET ImageUrl = '/images/webdev.png' 
WHERE ImageUrl LIKE '%Developer%';
```

### Вариант 2: Загрузить недостающие изображения на сервер

1. Найдите или создайте изображения для:
   - VDI продуктов (vdi_start_*.png, vdi_standrt_*.png, vdi_pro_*.png)
   - Hosting продуктов (hosting1.png, hosting2.png, hosting3.png)
   - NAS продуктов (nas1.png, nas2.png, nas3.png)
   - Builder продуктов (Builder1.png, Builder2.png, Builder3.png)
   - Developer продуктов (Developer*.png)

2. Скопируйте файлы на сервер в папку:
   `/home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/`

3. Установите права:
```bash
sudo chown -R siteadmin:siteadmin /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/
sudo chmod -R 644 /home/siteadmin/cloudcity/CloudCityCenter/wwwroot/images/*.png
```

### Вариант 3: Использовать placeholder изображения (временно)

Обновить пути на внешние placeholder изображения:

```sql
UPDATE Products 
SET ImageUrl = 'https://via.placeholder.com/300x200?text=VDI' 
WHERE ImageUrl LIKE '%vdi%';

UPDATE Products 
SET ImageUrl = 'https://via.placeholder.com/300x200?text=Hosting' 
WHERE ImageUrl LIKE '%hosting1%' OR ImageUrl LIKE '%hosting2%' OR ImageUrl LIKE '%hosting3%';

UPDATE Products 
SET ImageUrl = 'https://via.placeholder.com/300x200?text=NAS' 
WHERE ImageUrl LIKE '%nas%';

UPDATE Products 
SET ImageUrl = 'https://via.placeholder.com/300x200?text=Builder' 
WHERE ImageUrl LIKE '%Builder%';

UPDATE Products 
SET ImageUrl = 'https://via.placeholder.com/300x200?text=Developer' 
WHERE ImageUrl LIKE '%Developer%';
```

## Рекомендация: Вариант 1 (быстрое решение)

Используйте Вариант 1 - замените пути на существующие изображения. Это быстро решит проблему с 404 ошибками.

## Выполнение SQL запросов на сервере

### Через sqlcmd (если установлен):

```bash
sqlcmd -S 10.151.10.8 -U sa -P "St123T123$%" -d CloudCityDB -Q "UPDATE Products SET ImageUrl = '/images/hosting.png' WHERE ImageUrl LIKE '%vdi%';"
```

### Через .NET приложение:

Создайте временный скрипт или выполните через админ-панель, обновив ImageUrl для каждого продукта.

## Проверка после обновления

1. Проверьте логи на сервере - не должно быть 404 ошибок:
```bash
sudo journalctl -u cloudcity -f | grep "404"
```

2. Проверьте сайт в браузере - изображения должны отображаться

3. Проверьте в DevTools (F12) → Network → обновите страницу - запросы к изображениям должны возвращать 200

