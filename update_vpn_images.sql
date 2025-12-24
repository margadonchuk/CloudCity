-- =============================================
-- Скрипт обновления изображений для VPN товаров
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- Обновление изображений для VPN товаров
-- =============================================

UPDATE Products
SET ImageUrl = '/images/vpn1.png'
WHERE Slug = 'vpn-network' AND Type = 4;
GO

UPDATE Products
SET ImageUrl = '/images/vpn2.png'
WHERE Slug = 'vpn-device' AND Type = 4;
GO

UPDATE Products
SET ImageUrl = '/images/vpn3.png'
WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

-- =============================================
-- Готово! Изображения обновлены
-- =============================================

