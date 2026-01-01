-- =============================================
-- ОБНОВЛЕНИЕ ИЗОБРАЖЕНИЙ ДЛЯ ТОВАРОВ ХОСТИНГА
-- =============================================

USE CloudCityDB;
GO

-- Обновить изображение для Basic Hosting
UPDATE Products 
SET ImageUrl = '/images/hosting1.png'
WHERE Slug = 'basic-hosting' AND Type = 1;
GO

-- Обновить изображение для Business Hosting
UPDATE Products 
SET ImageUrl = '/images/hosting2.png'
WHERE Slug = 'business-hosting' AND Type = 1;
GO

-- Обновить изображение для Premium Hosting
UPDATE Products 
SET ImageUrl = '/images/hosting3.png'
WHERE Slug = 'premium-hosting' AND Type = 1;
GO

-- =============================================
-- ПРОВЕРКА РЕЗУЛЬТАТОВ
-- =============================================

-- Проверить изображения для всех товаров хостинга
SELECT Id, Name, Slug, ImageUrl 
FROM Products 
WHERE Type = 1
ORDER BY PricePerMonth;
GO

