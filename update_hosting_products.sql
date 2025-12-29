-- =============================================
-- Скрипт обновления товаров хостинга
-- Обновляет параметры: CPU, RAM, SSD (NVMe), OS, Bandwidth, Country
-- Работает для SQL Server и SQLite
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
USE CloudCityDB;
GO

-- =============================================
-- ШАГ 1: Обновить Location для товаров хостинга
-- =============================================
-- Обновляем Location на "Нидерланды" для всех товаров хостинга
UPDATE Products 
SET Location = 'Netherlands'
WHERE Type = 1;

-- =============================================
-- ШАГ 1.1: Найти ID товара "Basic Hosting"
-- =============================================
-- Выполните этот запрос, чтобы узнать ID:
-- SELECT Id, Name, Slug FROM Products WHERE Slug = 'basic-hosting' AND Type = 1;

-- =============================================
-- ШАГ 2: Удалить старые фичи для "Basic Hosting"
-- =============================================
-- Удаляем старые фичи (Storage, Email Accounts)
DELETE FROM ProductFeatures 
WHERE ProductId IN (
    SELECT Id FROM Products WHERE Slug = 'basic-hosting' AND Type = 1
)
AND Name IN ('Storage', 'Email Accounts', 'Трафик', 'Страна');

-- Обновить существующие записи "Трафик" на "Bandwidth" для всех товаров хостинга
UPDATE ProductFeatures
SET Name = 'Bandwidth'
WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 1)
AND Name = 'Трафик';

-- Обновить Location для всех товаров хостинга на "Netherlands"
UPDATE Products
SET Location = 'Netherlands'
WHERE Type = 1 AND (Location = 'Нидерланды' OR Location LIKE '%?%');

-- Обновить существующие записи "Страна" или "Гео" на "Country" для всех товаров хостинга
UPDATE ProductFeatures
SET Name = 'Country', Value = 'Netherlands'
WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 1)
AND (Name = 'Страна' OR Name = 'Гео');

-- =============================================
-- ШАГ 3: Добавить новые фичи для "Basic Hosting"
-- =============================================
-- ВАЖНО: Замените [PRODUCT_ID] на реальный ID из ШАГА 1
-- Или используйте подзапрос (работает для SQL Server и SQLite)

-- Вариант 1: С использованием подзапроса (рекомендуется)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'CPU' AS Name,
    '1 Core' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'CPU'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'RAM' AS Name,
    '1 GB' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'RAM'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'SSD (NVMe)' AS Name,
    '10 GB' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'SSD (NVMe)'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'OS' AS Name,
    'Linux' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'OS'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'Bandwidth' AS Name,
    '100 GB' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'Bandwidth'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    Id AS ProductId,
    'Country' AS Name,
    'Netherlands' AS Value
FROM Products 
WHERE Slug = 'basic-hosting' AND Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = Products.Id AND Name = 'Гео'
);

-- =============================================
-- ШАГ 4: Добавить новые товары хостинга (опционально)
-- =============================================

-- Premium Hosting
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 
    'Premium Hosting',
    'premium-hosting',
    1, -- ProductType.Hosting
    'Netherlands',
    25.00,
    'Shared hosting premium',
    1,
    1,
    '/images/hosting-premium.png'
WHERE NOT EXISTS (
    SELECT 1 FROM Products WHERE Slug = 'premium-hosting'
);

-- Добавляем фичи для Premium Hosting
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'CPU' AS Name,
    '2 Cores' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'CPU'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'RAM' AS Name,
    '2 GB' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'RAM'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'SSD (NVMe)' AS Name,
    '50 GB' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'SSD (NVMe)'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'OS' AS Name,
    'Linux' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'OS'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'Bandwidth' AS Name,
    '500 GB' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'Bandwidth'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'Country' AS Name,
    'Netherlands' AS Value
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'Гео'
);

-- Business Hosting
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 
    'Business Hosting',
    'business-hosting',
    1, -- ProductType.Hosting
    'Netherlands',
    50.00,
    'Shared hosting business',
    1,
    1,
    '/images/hosting-business.png'
WHERE NOT EXISTS (
    SELECT 1 FROM Products WHERE Slug = 'business-hosting'
);

-- Добавляем фичи для Business Hosting
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'CPU' AS Name,
    '4 Cores' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'CPU'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'RAM' AS Name,
    '4 GB' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'RAM'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'SSD (NVMe)' AS Name,
    '100 GB' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'SSD (NVMe)'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'OS' AS Name,
    'Linux / Windows' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'OS'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'Bandwidth' AS Name,
    '1 TB' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'Bandwidth'
);

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT 
    p.Id AS ProductId,
    'Country' AS Name,
    'Netherlands' AS Value
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductFeatures 
    WHERE ProductId = p.Id AND Name = 'Гео'
);

-- =============================================
-- ШАГ 5: Добавить варианты оплаты (ProductVariants)
-- =============================================

-- Варианты для Basic Hosting
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT 
    p.Id AS ProductId,
    'Monthly' AS Name,
    10.00 AS Price,
    0 AS BillingPeriod -- BillingPeriod.Monthly = 0
FROM Products p
WHERE p.Slug = 'basic-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductVariants 
    WHERE ProductId = p.Id AND Name = 'Monthly'
);

-- Варианты для Premium Hosting
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT 
    p.Id AS ProductId,
    'Monthly' AS Name,
    25.00 AS Price,
    0 AS BillingPeriod
FROM Products p
WHERE p.Slug = 'premium-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductVariants 
    WHERE ProductId = p.Id AND Name = 'Monthly'
);

-- Варианты для Business Hosting
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT 
    p.Id AS ProductId,
    'Monthly' AS Name,
    50.00 AS Price,
    0 AS BillingPeriod
FROM Products p
WHERE p.Slug = 'business-hosting' AND p.Type = 1
AND NOT EXISTS (
    SELECT 1 FROM ProductVariants 
    WHERE ProductId = p.Id AND Name = 'Monthly'
);

-- =============================================
-- Проверка результатов
-- =============================================
-- Выполните эти запросы для проверки:

-- Проверить все товары хостинга
-- SELECT Id, Name, Slug, PricePerMonth FROM Products WHERE Type = 1;

-- Проверить фичи для Basic Hosting
-- SELECT pf.Name, pf.Value 
-- FROM ProductFeatures pf
-- INNER JOIN Products p ON pf.ProductId = p.Id
-- WHERE p.Slug = 'basic-hosting' AND p.Type = 1
-- ORDER BY 
--     CASE pf.Name
--         WHEN 'CPU' THEN 1
--         WHEN 'RAM' THEN 2
--         WHEN 'SSD (NVMe)' THEN 3
--         WHEN 'OS' THEN 4
--         WHEN 'Bandwidth' THEN 5
--         WHEN 'Country' THEN 6
--         ELSE 99
--     END;

