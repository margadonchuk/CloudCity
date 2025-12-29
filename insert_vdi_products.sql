-- =============================================
-- Скрипт добавления VDI товаров в базу данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- ШАГ 1: Добавление VDI товаров
-- =============================================

-- Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - Netherlands', 'vdi-start-netherlands', 8, 'Netherlands', 150.00, 'VDI на 1 человека - Start', 1, 1, '/images/vdi_start_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-netherlands');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - Netherlands', 'vdi-standard-netherlands', 8, 'Netherlands', 175.00, 'VDI на 1 человека - Standard', 1, 1, '/images/vdi_standrt_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-netherlands');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - Netherlands', 'vdi-pro-netherlands', 8, 'Netherlands', 200.00, 'VDI на 1 человека - Pro', 1, 1, '/images/vdi_pro_1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-netherlands');
GO

-- Germany/France/Poland
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - Germany/France/Poland', 'vdi-start-europe', 8, 'Germany/France/Poland', 155.00, 'VDI на 1 человека - Start', 1, 1, '/images/vdi_start_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-europe');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - Germany/France/Poland', 'vdi-standard-europe', 8, 'Germany/France/Poland', 180.00, 'VDI на 1 человека - Standard', 1, 1, '/images/vdi_standrt_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-europe');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - Germany/France/Poland', 'vdi-pro-europe', 8, 'Germany/France/Poland', 205.00, 'VDI на 1 человека - Pro', 1, 1, '/images/vdi_pro_2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-europe');
GO

-- USA/Canada/Asia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Start - USA/Canada/Asia', 'vdi-start-usa-asia', 8, 'USA/Canada/Asia', 175.00, 'VDI на 1 человека - Start', 1, 1, '/images/vdi_start_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-start-usa-asia');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Standard - USA/Canada/Asia', 'vdi-standard-usa-asia', 8, 'USA/Canada/Asia', 200.00, 'VDI на 1 человека - Standard', 1, 1, '/images/vdi_standrt_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-standard-usa-asia');
GO

INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VDI Pro - USA/Canada/Asia', 'vdi-pro-usa-asia', 8, 'USA/Canada/Asia', 225.00, 'VDI на 1 человека - Pro', 1, 1, '/images/vdi_pro_3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vdi-pro-usa-asia');
GO

-- =============================================
-- ШАГ 2: Добавление вариантов товаров (ProductVariants)
-- =============================================

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 150.00, 0 FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 175.00, 0 FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 200.00, 0 FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 155.00, 0 FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 180.00, 0 FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 205.00, 0 FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 175.00, 0 FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 200.00, 0 FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia'));
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 225.00, 0 FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia'));
GO

-- =============================================
-- ШАГ 3: Добавление характеристик товаров (ProductFeatures)
-- =============================================

-- VDI Start - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-start-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-netherlands') AND Name = 'Country');
GO

-- VDI Standard - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-standard-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-netherlands') AND Name = 'Country');
GO

-- VDI Pro - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'vdi-pro-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-netherlands') AND Name = 'Country');
GO

-- VDI Start - Germany/France/Poland
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-start-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-europe') AND Name = 'Country');
GO

-- VDI Standard - Germany/France/Poland
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-standard-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-europe') AND Name = 'Country');
GO

-- VDI Pro - Germany/France/Poland
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany/France/Poland' FROM Products WHERE Slug = 'vdi-pro-europe'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-europe') AND Name = 'Country');
GO

-- VDI Start - USA/Canada/Asia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-start-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-start-usa-asia') AND Name = 'Country');
GO

-- VDI Standard - USA/Canada/Asia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-standard-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-standard-usa-asia') AND Name = 'Country');
GO

-- VDI Pro - USA/Canada/Asia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb/s' FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'USA/Canada/Asia' FROM Products WHERE Slug = 'vdi-pro-usa-asia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vdi-pro-usa-asia') AND Name = 'Country');
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
    COUNT(DISTINCT pf.Id) AS FeaturesCount,
    COUNT(DISTINCT pv.Id) AS VariantsCount
FROM Products p
LEFT JOIN ProductFeatures pf ON p.Id = pf.ProductId
LEFT JOIN ProductVariants pv ON p.Id = pv.ProductId
WHERE p.Type = 8
GROUP BY p.Id, p.Name, p.Slug, p.Type, p.Location, p.PricePerMonth
ORDER BY p.Location, p.PricePerMonth;
GO
