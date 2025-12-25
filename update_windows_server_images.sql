-- =============================================
-- Скрипт обновления изображений для Windows Server товаров
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- Обновление изображений для Windows Server 5-8 человек
-- =============================================

-- Windows Server 5-8 - Netherlands
UPDATE Products 
SET ImageUrl = '/images/nether5.png'
WHERE Slug = 'windows-server-5-8-netherlands';
GO

-- Windows Server 5-8 - Germany
UPDATE Products 
SET ImageUrl = '/images/germ5.png'
WHERE Slug = 'windows-server-5-8-germany';
GO

-- Windows Server 5-8 - Canada/USA
UPDATE Products 
SET ImageUrl = '/images/usa5.png'
WHERE Slug = 'windows-server-5-8-canada-usa';
GO

-- Windows Server 5-8 - UK/Poland/France
UPDATE Products 
SET ImageUrl = '/images/pl5.png'
WHERE Slug = 'windows-server-5-8-uk-pol-fr';
GO

-- Windows Server 5-8 - Singapore/India/Australia
UPDATE Products 
SET ImageUrl = '/images/ind5.png'
WHERE Slug = 'windows-server-5-8-singapore-india-australia';
GO

-- =============================================
-- Обновление изображений для Windows Server 15 человек
-- =============================================

-- Windows Server 15 - Netherlands
UPDATE Products 
SET ImageUrl = '/images/nether15.png'
WHERE Slug = 'windows-server-15-netherlands';
GO

-- Windows Server 15 - Germany
UPDATE Products 
SET ImageUrl = '/images/germ15.png'
WHERE Slug = 'windows-server-15-germany';
GO

-- Windows Server 15 - Canada/USA
UPDATE Products 
SET ImageUrl = '/images/usa15.png'
WHERE Slug = 'windows-server-15-canada-usa';
GO

-- Windows Server 15 - UK/Poland/France
UPDATE Products 
SET ImageUrl = '/images/pl15.png'
WHERE Slug = 'windows-server-15-uk-pol-fr';
GO

-- Windows Server 15 - Singapore/India/Australia
UPDATE Products 
SET ImageUrl = '/images/ind15.png'
WHERE Slug = 'windows-server-15-singapore-india-australia';
GO

-- =============================================
-- Обновление изображений для Windows Server 25 человек
-- =============================================

-- Windows Server 25 - Netherlands
UPDATE Products 
SET ImageUrl = '/images/nether25.png'
WHERE Slug = 'windows-server-25-netherlands';
GO

-- Windows Server 25 - Germany
UPDATE Products 
SET ImageUrl = '/images/germ25.png'
WHERE Slug = 'windows-server-25-germany';
GO

-- Windows Server 25 - Canada/USA
UPDATE Products 
SET ImageUrl = '/images/usa25.png'
WHERE Slug = 'windows-server-25-canada-usa';
GO

-- Windows Server 25 - UK/Poland/France
UPDATE Products 
SET ImageUrl = '/images/pl25.png'
WHERE Slug = 'windows-server-25-uk-pol-fr';
GO

-- Windows Server 25 - Singapore/India/Australia
UPDATE Products 
SET ImageUrl = '/images/ind25.png'
WHERE Slug = 'windows-server-25-singapore-india-australia';
GO

-- =============================================
-- Проверка результатов
-- =============================================

-- Проверьте что все изображения обновлены
SELECT 
    Name,
    Slug,
    Location,
    ImageUrl
FROM Products 
WHERE Type = 0
ORDER BY 
    CASE 
        WHEN Configuration LIKE '%5-8%' THEN 1
        WHEN Configuration LIKE '%15%' THEN 2
        WHEN Configuration LIKE '%25%' THEN 3
        WHEN Configuration LIKE '%35%' THEN 4
        WHEN Configuration LIKE '%50%' THEN 5
        ELSE 6
    END,
    Location;
GO


