-- =============================================
-- Скрипт добавления VDI товаров для 10 человек в базу данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- ШАГ 1: Добавление VDI товаров для 10 человек
-- =============================================

-- Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - Netherlands (10 persons)', 'vdi-start-netherlands-10', 8, 'Netherlands', 90.00, 'VDI на 10 человек - Start', 1, 1, '/images/vdi_start_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-netherlands-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - Netherlands (10 persons)', 'vdi-standard-netherlands-10', 8, 'Netherlands', 115.00, 'VDI на 10 человек - Standard', 1, 1, '/images/vdi_standrt_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-netherlands-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - Netherlands (10 persons)', 'vdi-pro-netherlands-10', 8, 'Netherlands', 140.00, 'VDI на 10 человек - Pro', 1, 1, '/images/vdi_pro_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-netherlands-10');
GO

-- Germany/France/Poland
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - Germany/France/Poland (10 persons)', 'vdi-start-europe-10', 8, 'Germany/France/Poland', 95.00, 'VDI на 10 человек - Start', 1, 1, '/images/vdi_start_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-europe-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - Germany/France/Poland (10 persons)', 'vdi-standard-europe-10', 8, 'Germany/France/Poland', 120.00, 'VDI на 10 человек - Standard', 1, 1, '/images/vdi_standrt_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-europe-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - Germany/France/Poland (10 persons)', 'vdi-pro-europe-10', 8, 'Germany/France/Poland', 145.00, 'VDI на 10 человек - Pro', 1, 1, '/images/vdi_pro_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-europe-10');
GO

-- USA/Canada/Asia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - USA/Canada/Asia (10 persons)', 'vdi-start-usa-asia-10', 8, 'USA/Canada/Asia', 115.00, 'VDI на 10 человек - Start', 1, 1, '/images/vdi_start_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-usa-asia-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - USA/Canada/Asia (10 persons)', 'vdi-standard-usa-asia-10', 8, 'USA/Canada/Asia', 140.00, 'VDI на 10 человек - Standard', 1, 1, '/images/vdi_standrt_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-usa-asia-10');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - USA/Canada/Asia (10 persons)', 'vdi-pro-usa-asia-10', 8, 'USA/Canada/Asia', 165.00, 'VDI на 10 человек - Pro', 1, 1, '/images/vdi_pro_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-usa-asia-10');
GO

-- =============================================
-- ШАГ 2: Добавление вариантов товаров (ProductVariants)
-- =============================================

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 90.00, 0 FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 115.00, 0 FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 140.00, 0 FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 95.00, 0 FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 120.00, 0 FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 145.00, 0 FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 115.00, 0 FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 140.00, 0 FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 165.00, 0 FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'));
GO

-- =============================================
-- ШАГ 3: Добавление характеристик товаров (ProductFeatures)
-- =============================================

-- VDI Start - Netherlands (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-start-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands-10') AND Name = 'Country');
GO

-- VDI Standard - Netherlands (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-standard-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands-10') AND Name = 'Country');
GO

-- VDI Pro - Netherlands (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-pro-netherlands-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands-10') AND Name = 'Country');
GO

-- VDI Start - Germany/France/Poland (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-start-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe-10') AND Name = 'Country');
GO

-- VDI Standard - Germany/France/Poland (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-standard-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe-10') AND Name = 'Country');
GO

-- VDI Pro - Germany/France/Poland (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-pro-europe-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe-10') AND Name = 'Country');
GO

-- VDI Start - USA/Canada/Asia (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-start-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia-10') AND Name = 'Country');
GO

-- VDI Standard - USA/Canada/Asia (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-standard-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia-10') AND Name = 'Country');
GO

-- VDI Pro - USA/Canada/Asia (10 persons)
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-pro-usa-asia-10'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia-10') AND Name = 'Country');
GO

-- =============================================
-- Проверка добавленных товаров
-- =============================================
SELECT 
    p.Id,
    p.Name,
    p.Slug,
    p.Type,
    p.Location,
    p.PricePerMonth,
    p.Configuration,
    COUNT(DISTINCT pf.Id) AS FeaturesCount,
    COUNT(DISTINCT pv.Id) AS VariantsCount
FROM Products p
LEFT JOIN ProductFeatures pf ON p.Id = pf.ProductId
LEFT JOIN ProductVariants pv ON p.Id = pv.ProductId
WHERE p.Type = 8 AND p.Configuration LIKE '%10 человек%'
GROUP BY p.Id, p.Name, p.Slug, p.Type, p.Location, p.PricePerMonth, p.Configuration
ORDER BY p.Location, p.PricePerMonth;
GO

