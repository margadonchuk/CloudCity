-- SQL скрипт для обновления путей к изображениям в базе данных
-- Выполните на сервере Ubuntu через sqlcmd или подключитесь к SQL Server Management Studio

USE CloudCityDB;
GO

-- Обновить VDI продукты - использовать hosting.png
UPDATE Products 
SET ImageUrl = '/images/hosting.png' 
WHERE ImageUrl LIKE '%vdi%';

-- Обновить Hosting продукты (hosting1, hosting2, hosting3) - использовать hosting.png
UPDATE Products 
SET ImageUrl = '/images/hosting.png' 
WHERE ImageUrl LIKE '%hosting1%' OR ImageUrl LIKE '%hosting2%' OR ImageUrl LIKE '%hosting3%';

-- Обновить NAS продукты - использовать storage.png
UPDATE Products 
SET ImageUrl = '/images/storage.png' 
WHERE ImageUrl LIKE '%nas%';

-- Обновить Builder продукты - использовать webdev.png
UPDATE Products 
SET ImageUrl = '/images/webdev.png' 
WHERE ImageUrl LIKE '%Builder%';

-- Обновить Developer продукты - использовать webdev.png
UPDATE Products 
SET ImageUrl = '/images/webdev.png' 
WHERE ImageUrl LIKE '%Developer%';

-- Проверить результат
SELECT Id, Name, ImageUrl 
FROM Products 
WHERE ImageUrl LIKE '%vdi%' 
   OR ImageUrl LIKE '%hosting1%' 
   OR ImageUrl LIKE '%hosting2%' 
   OR ImageUrl LIKE '%hosting3%'
   OR ImageUrl LIKE '%nas%'
   OR ImageUrl LIKE '%Builder%'
   OR ImageUrl LIKE '%Developer%';
GO

