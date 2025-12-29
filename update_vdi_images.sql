-- =============================================
-- Скрипт обновления изображений для VDI товаров
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- Обновление изображений для VDI товаров
-- =============================================

-- Netherlands
UPDATE Products 
SET ImageUrl = '/images/vdi_start_1.png'
WHERE Slug = 'vdi-start-netherlands' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt_1.png'
WHERE Slug = 'vdi-standard-netherlands' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_pro_1.png'
WHERE Slug = 'vdi-pro-netherlands' AND Type = 8;
GO

-- Germany/France/Poland
UPDATE Products 
SET ImageUrl = '/images/vdi_start_2.png'
WHERE Slug = 'vdi-start-europe' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt_2.png'
WHERE Slug = 'vdi-standard-europe' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_pro_2.png'
WHERE Slug = 'vdi-pro-europe' AND Type = 8;
GO

-- USA/Canada/Asia
UPDATE Products 
SET ImageUrl = '/images/vdi_start_3.png'
WHERE Slug = 'vdi-start-usa-asia' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt_3.png'
WHERE Slug = 'vdi-standard-usa-asia' AND Type = 8;
GO

UPDATE Products 
SET ImageUrl = '/images/vdi_pro_3.png'
WHERE Slug = 'vdi-pro-usa-asia' AND Type = 8;
GO

-- =============================================
-- Проверка обновленных изображений
-- =============================================
SELECT 
    p.Id,
    p.Name,
    p.Slug,
    p.Location,
    p.ImageUrl
FROM Products p
WHERE p.Type = 8
ORDER BY p.Location, p.PricePerMonth;
GO

