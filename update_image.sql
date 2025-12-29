-- Скрипт для изменения картинок товаров в базе данных
-- Используйте этот скрипт если хотите изменить путь к картинке или использовать внешний URL

USE CloudCityDB;
GO

-- Пример 1: Изменить картинку для конкретного товара
UPDATE Products 
SET ImageUrl = '/images/ds1-new.png' 
WHERE Name = 'DS1-E3';
GO

-- Пример 2: Использовать внешний URL
-- UPDATE Products 
-- SET ImageUrl = 'https://example.com/image.png' 
-- WHERE Name = 'DS1-E3';
-- GO

-- Пример 3: Изменить все Dedicated Servers
-- UPDATE Products 
-- SET ImageUrl = '/images/new-server.png' 
-- WHERE Type = 0;  -- 0 = DedicatedServer
-- GO

-- Пример 4: Изменить все VPS
-- UPDATE Products 
-- SET ImageUrl = '/images/new-vps.png' 
-- WHERE Type = 3;  -- 3 = VPS
-- GO

-- Проверка результата
SELECT Name, ImageUrl FROM Products WHERE Name = 'DS1-E3';
GO

