-- =============================================
-- Скрипт добавления/обновления VPS товаров в базу данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- ШАГ 1: Удаление старых VPS товаров (если нужно обновить)
-- =============================================

-- Удаляем старые VPS продукты (можно закомментировать, если хотите сохранить старые)
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 3 AND Slug IN ('vps1-1-20', 'vps2-2-20', 'vps3-4-40', 'vps4-4-80', 'vps5-4-80', 'vps6-6-100'));
GO

DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 3 AND Slug IN ('vps1-1-20', 'vps2-2-20', 'vps3-4-40', 'vps4-4-80', 'vps5-4-80', 'vps6-6-100'));
GO

DELETE FROM Products WHERE Type = 3 AND Slug IN ('vps1-1-20', 'vps2-2-20', 'vps3-4-40', 'vps4-4-80', 'vps5-4-80', 'vps6-6-100');
GO

-- =============================================
-- ШАГ 2: Добавление новых VPS товаров
-- =============================================

-- VPS1-1-20
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS1-1-20', 'vps1-1-20', 3, 'Netherlands/Germany/France', 10.00, '1 Core, 1GB RAM, 20GB SSD', 1, 1, '/images/vps1.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps1-1-20');
GO

-- VPS2-2-40
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS2-2-40', 'vps2-2-40', 3, 'Netherlands/Germany/France', 20.00, '2 Cores, 2GB RAM, 40GB SSD', 1, 1, '/images/vps2.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps2-2-40');
GO

-- VPS3-2-150
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS3-2-150', 'vps3-2-150', 3, 'Netherlands/Germany/France', 35.00, '2 Cores, 4GB RAM, 150GB SSD', 1, 1, '/images/vps3.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps3-2-150');
GO

-- VPS4-4-80
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS4-4-80', 'vps4-4-80', 3, 'Netherlands/Germany/France', 49.00, '4 Cores, 4GB RAM, 80GB SSD', 1, 1, '/images/vps4.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps4-4-80');
GO

-- VPS5-6-100
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS5-6-100', 'vps5-6-100', 3, 'Netherlands/Germany/France', 80.00, '6 Cores, 12GB RAM, 100GB SSD', 1, 1, '/images/vps5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps5-6-100');
GO

-- VPS6-8-120
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS6-8-120', 'vps6-8-120', 3, 'Netherlands/Germany/France', 95.00, '8 Cores, 8GB RAM, 120GB SSD', 1, 1, '/images/vps6.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps6-8-120');
GO

-- VPS7-8-160
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS7-8-160', 'vps7-8-160', 3, 'Netherlands/Germany/France', 150.00, '8 Cores, 16GB RAM, 160GB SSD', 1, 1, '/images/vps7.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps7-8-160');
GO

-- VPS8-8-200
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS8-8-200', 'vps8-8-200', 3, 'Netherlands/Germany/France', 175.00, '8 Cores, 24GB RAM, 200GB SSD', 1, 1, '/images/vps8.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps8-8-200');
GO

-- VPS9-12-300
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS9-12-300', 'vps9-12-300', 3, 'Netherlands/Germany/France', 220.00, '12 Cores, 48GB RAM, 300GB SSD', 1, 1, '/images/vps9.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps9-12-300');
GO

-- VPS10-16-320
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPS10-16-320', 'vps10-16-320', 3, 'Netherlands/Germany/France', 249.00, '16 Cores, 32GB RAM, 320GB SSD', 1, 1, '/images/vps10.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vps10-16-320');
GO

-- =============================================
-- ШАГ 3: Добавление вариантов товаров (ProductVariants)
-- =============================================

-- VPS1-1-20
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 10.00, 0 FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20'));
GO

-- VPS2-2-40
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 20.00, 0 FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40'));
GO

-- VPS3-2-150
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 35.00, 0 FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150'));
GO

-- VPS4-4-80
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 49.00, 0 FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80'));
GO

-- VPS5-6-100
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 80.00, 0 FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100'));
GO

-- VPS6-8-120
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 95.00, 0 FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120'));
GO

-- VPS7-8-160
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 150.00, 0 FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160'));
GO

-- VPS8-8-200
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 175.00, 0 FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200'));
GO

-- VPS9-12-300
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 220.00, 0 FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300'));
GO

-- VPS10-16-320
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 249.00, 0 FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320'));
GO

-- =============================================
-- ШАГ 4: Добавление характеристик товаров (ProductFeatures)
-- =============================================

-- VPS1-1-20
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '1 core' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '1 GB' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '20 GB' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '100 mb' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps1-1-20'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps1-1-20') AND Name = 'OS');
GO

-- VPS2-2-40
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '2 core' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '2 GB' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '40 GB' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '250 mb' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps2-2-40'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps2-2-40') AND Name = 'OS');
GO

-- VPS3-2-150
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '2 core' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '4 GB' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '250 mb' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps3-2-150'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps3-2-150') AND Name = 'OS');
GO

-- VPS4-4-80
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '4 core' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '4 GB' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '80 GB' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '500 mb' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps4-4-80'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps4-4-80') AND Name = 'OS');
GO

-- VPS5-6-100
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '6 core' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '12 GB' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '100 GB' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps5-6-100'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps5-6-100') AND Name = 'OS');
GO

-- VPS6-8-120
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '8 GB' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '120 GB' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '500 mb' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps6-8-120'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps6-8-120') AND Name = 'OS');
GO

-- VPS7-8-160
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '16 GB' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '160 GB' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps7-8-160'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps7-8-160') AND Name = 'OS');
GO

-- VPS8-8-200
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '8 core' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '24 GB' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '200 GB' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps8-8-200'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps8-8-200') AND Name = 'OS');
GO

-- VPS9-12-300
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12 core' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '48 GB' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '300 GB' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps9-12-300'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps9-12-300') AND Name = 'OS');
GO

-- VPS10-16-320
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '16 core' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '32 GB' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '320 GB' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', '1 Gb' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'Traffic');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'Country');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'OS', 'Linux' FROM Products WHERE Slug = 'vps10-16-320'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'vps10-16-320') AND Name = 'OS');
GO

-- =============================================
-- Готово! Все VPS товары добавлены в базу данных
-- =============================================

