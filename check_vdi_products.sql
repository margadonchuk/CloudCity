-- =============================================
-- Скрипт проверки VDI товаров в базе данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- USE CloudCityDB;
-- GO

-- Проверка всех VDI товаров
SELECT 
    p.Id,
    p.Name,
    p.Slug,
    p.Type,
    p.Location,
    p.PricePerMonth,
    p.Configuration,
    p.IsPublished,
    p.ImageUrl,
    COUNT(DISTINCT pf.Id) AS FeaturesCount,
    COUNT(DISTINCT pv.Id) AS VariantsCount
FROM Products p
LEFT JOIN ProductFeatures pf ON p.Id = pf.ProductId
LEFT JOIN ProductVariants pv ON p.Id = pv.ProductId
WHERE p.Type = 8
GROUP BY p.Id, p.Name, p.Slug, p.Type, p.Location, p.PricePerMonth, p.Configuration, p.IsPublished, p.ImageUrl
ORDER BY p.Configuration, p.Location, p.PricePerMonth;
GO

-- Проверка товаров для 1 человека
SELECT 
    p.Id,
    p.Name,
    p.Slug,
    p.Configuration,
    p.Location,
    p.PricePerMonth
FROM Products p
WHERE p.Type = 8 
AND (p.Configuration LIKE '%1 человека%' OR p.Configuration LIKE '%1 person%' OR (p.Configuration NOT LIKE '%3%' AND p.Slug NOT LIKE '%-3%'))
ORDER BY p.Location, p.PricePerMonth;
GO

-- Проверка товаров для 3 человек
SELECT 
    p.Id,
    p.Name,
    p.Slug,
    p.Configuration,
    p.Location,
    p.PricePerMonth
FROM Products p
WHERE p.Type = 8 
AND (p.Configuration LIKE '%3 человека%' OR p.Configuration LIKE '%3 persons%' OR p.Slug LIKE '%-3%')
ORDER BY p.Location, p.PricePerMonth;
GO

