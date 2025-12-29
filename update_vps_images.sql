-- =============================================
-- Скрипт обновления изображений для VPS товаров
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- Обновление изображений для VPS товаров
-- =============================================

UPDATE Products
SET ImageUrl = '/images/vps_1-1-20.png'
WHERE Slug = 'vps1-1-20' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_2-2-40.png'
WHERE Slug = 'vps2-2-40' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_3-2-150.png'
WHERE Slug = 'vps3-2-150' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_4-4-80.png'
WHERE Slug = 'vps4-4-80' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_5-6-100.png'
WHERE Slug = 'vps5-6-100' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_6-8-120.png'
WHERE Slug = 'vps6-8-120' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_7-8-160.png'
WHERE Slug = 'vps7-8-160' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_8-8-200.png'
WHERE Slug = 'vps8-8-200' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_9-12-300.png'
WHERE Slug = 'vps9-12-300' AND Type = 3;
GO

UPDATE Products
SET ImageUrl = '/images/vps_10-16-320.png'
WHERE Slug = 'vps10-16-320' AND Type = 3;
GO

-- =============================================
-- Готово! Изображения обновлены
-- =============================================

