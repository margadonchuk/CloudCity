-- =============================================
-- Скрипт добавления/обновления Windows Server товаров в базу данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- ШАГ 1: Удаление старых Windows Server товаров
-- =============================================

-- Удаляем старые Windows Server продукты (Type = 0 = DedicatedServer)
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 0);
GO

DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 0);
GO

DELETE FROM Products WHERE Type = 0;
GO

-- =============================================
-- ШАГ 2: Добавление новых Windows Server товаров
-- =============================================

-- Windows Server на 5-8 человек - Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 5-8 - Netherlands', 'windows-server-5-8-netherlands', 0, 'Netherlands', 250.00, 'Windows Server на 5-8 человек', 1, 1, '/images/nether5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-5-8-netherlands');
GO

-- Windows Server на 5-8 человек - Germany
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 5-8 - Germany', 'windows-server-5-8-germany', 0, 'Germany', 350.00, 'Windows Server на 5-8 человек', 1, 1, '/images/germ5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-5-8-germany');
GO

-- Windows Server на 5-8 человек - United Kingdom / Poland / France
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 5-8 - UK/Poland/France', 'windows-server-5-8-uk-pol-fr', 0, 'United Kingdom / Poland / France', 400.00, 'Windows Server на 5-8 человек', 1, 1, '/images/pl5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr');
GO

-- Windows Server на 5-8 человек - Canada/ USA
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 5-8 - Canada/USA', 'windows-server-5-8-canada-usa', 0, 'Canada/ USA', 350.00, 'Windows Server на 5-8 человек', 1, 1, '/images/usa5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-5-8-canada-usa');
GO

-- Windows Server на 5-8 человек - Singapore / India / Australia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 5-8 - Singapore/India/Australia', 'windows-server-5-8-singapore-india-australia', 0, 'Singapore / India / Australia', 450.00, 'Windows Server на 5-8 человек', 1, 1, '/images/ind5.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia');
GO

-- Windows Server на 15 человек - Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 15 - Netherlands', 'windows-server-15-netherlands', 0, 'Netherlands', 440.00, 'Windows Server на 15 человек', 1, 1, '/images/nether15.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-15-netherlands');
GO

-- Windows Server на 15 человек - Germany
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 15 - Germany', 'windows-server-15-germany', 0, 'Germany', 540.00, 'Windows Server на 15 человек', 1, 1, '/images/germ15.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-15-germany');
GO

-- Windows Server на 15 человек - United Kingdom / Poland / France
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 15 - UK/Poland/France', 'windows-server-15-uk-pol-fr', 0, 'United Kingdom / Poland / France', 590.00, 'Windows Server на 15 человек', 1, 1, '/images/pl15.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr');
GO

-- Windows Server на 15 человек - Canada/ USA
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 15 - Canada/USA', 'windows-server-15-canada-usa', 0, 'Canada/ USA', 540.00, 'Windows Server на 15 человек', 1, 1, '/images/usa15.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-15-canada-usa');
GO

-- Windows Server на 15 человек - Singapore / India / Australia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 15 - Singapore/India/Australia', 'windows-server-15-singapore-india-australia', 0, 'Singapore / India / Australia', 640.00, 'Windows Server на 15 человек', 1, 1, '/images/ind15.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia');
GO

-- Windows Server на 25 человек - Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 25 - Netherlands', 'windows-server-25-netherlands', 0, 'Netherlands', 635.00, 'Windows Server на 25 человек', 1, 1, '/images/nether25.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-25-netherlands');
GO

-- Windows Server на 25 человек - Germany
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 25 - Germany', 'windows-server-25-germany', 0, 'Germany', 735.00, 'Windows Server на 25 человек', 1, 1, '/images/germ25.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-25-germany');
GO

-- Windows Server на 25 человек - United Kingdom / Poland / France
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 25 - UK/Poland/France', 'windows-server-25-uk-pol-fr', 0, 'United Kingdom / Poland / France', 785.00, 'Windows Server на 25 человек', 1, 1, '/images/pl25.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr');
GO

-- Windows Server на 25 человек - Canada/ USA
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 25 - Canada/USA', 'windows-server-25-canada-usa', 0, 'Canada/ USA', 735.00, 'Windows Server на 25 человек', 1, 1, '/images/usa25.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-25-canada-usa');
GO

-- Windows Server на 25 человек - Singapore / India / Australia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 25 - Singapore/India/Australia', 'windows-server-25-singapore-india-australia', 0, 'Singapore / India / Australia', 835.00, 'Windows Server на 25 человек', 1, 1, '/images/ind25.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia');
GO

-- Windows Server на 35 человек - Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 35 - Netherlands', 'windows-server-35-netherlands', 0, 'Netherlands', 825.00, 'Windows Server на 35 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-35-netherlands');
GO

-- Windows Server на 35 человек - Germany
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 35 - Germany', 'windows-server-35-germany', 0, 'Germany', 925.00, 'Windows Server на 35 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-35-germany');
GO

-- Windows Server на 35 человек - United Kingdom / Poland / France
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 35 - UK/Poland/France', 'windows-server-35-uk-pol-fr', 0, 'United Kingdom / Poland / France', 975.00, 'Windows Server на 35 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr');
GO

-- Windows Server на 35 человек - Canada/ USA
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 35 - Canada/USA', 'windows-server-35-canada-usa', 0, 'Canada/ USA', 925.00, 'Windows Server на 35 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-35-canada-usa');
GO

-- Windows Server на 35 человек - Singapore / India / Australia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 35 - Singapore/India/Australia', 'windows-server-35-singapore-india-australia', 0, 'Singapore / India / Australia', 1025.00, 'Windows Server на 35 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia');
GO

-- Windows Server на 50 человек - Netherlands
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 50 - Netherlands', 'windows-server-50-netherlands', 0, 'Netherlands', 1080.00, 'Windows Server на 50 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-50-netherlands');
GO

-- Windows Server на 50 человек - Germany
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 50 - Germany', 'windows-server-50-germany', 0, 'Germany', 1180.00, 'Windows Server на 50 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-50-germany');
GO

-- Windows Server на 50 человек - United Kingdom / Poland / France
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 50 - UK/Poland/France', 'windows-server-50-uk-pol-fr', 0, 'United Kingdom / Poland / France', 1230.00, 'Windows Server на 50 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr');
GO

-- Windows Server на 50 человек - Canada/ USA
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 50 - Canada/USA', 'windows-server-50-canada-usa', 0, 'Canada/ USA', 1180.00, 'Windows Server на 50 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-50-canada-usa');
GO

-- Windows Server на 50 человек - Singapore / India / Australia
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'Windows Server 50 - Singapore/India/Australia', 'windows-server-50-singapore-india-australia', 0, 'Singapore / India / Australia', 1280.00, 'Windows Server на 50 человек', 1, 1, '/images/windows-server.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia');
GO

-- =============================================
-- ШАГ 3: Добавление вариантов товаров (ProductVariants)
-- =============================================

-- Windows Server на 5-8 человек - Netherlands
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 250.00, 0 FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands'));
GO

-- Windows Server на 5-8 человек - Germany
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 350.00, 0 FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany'));
GO

-- Windows Server на 5-8 человек - UK/Poland/France
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 400.00, 0 FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'));
GO

-- Windows Server на 5-8 человек - Canada/USA
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 350.00, 0 FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'));
GO

-- Windows Server на 5-8 человек - Singapore/India/Australia
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 450.00, 0 FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'));
GO

-- Windows Server на 15 человек - Netherlands
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 440.00, 0 FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands'));
GO

-- Windows Server на 15 человек - Germany
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 540.00, 0 FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany'));
GO

-- Windows Server на 15 человек - UK/Poland/France
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 590.00, 0 FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'));
GO

-- Windows Server на 15 человек - Canada/USA
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 540.00, 0 FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa'));
GO

-- Windows Server на 15 человек - Singapore/India/Australia
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 640.00, 0 FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'));
GO

-- Windows Server на 25 человек - Netherlands
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 635.00, 0 FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands'));
GO

-- Windows Server на 25 человек - Germany
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 735.00, 0 FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany'));
GO

-- Windows Server на 25 человек - UK/Poland/France
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 785.00, 0 FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'));
GO

-- Windows Server на 25 человек - Canada/USA
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 735.00, 0 FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa'));
GO

-- Windows Server на 25 человек - Singapore/India/Australia
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 835.00, 0 FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'));
GO

-- Windows Server на 35 человек - Netherlands
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 825.00, 0 FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands'));
GO

-- Windows Server на 35 человек - Germany
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 925.00, 0 FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany'));
GO

-- Windows Server на 35 человек - UK/Poland/France
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 975.00, 0 FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'));
GO

-- Windows Server на 35 человек - Canada/USA
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 925.00, 0 FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa'));
GO

-- Windows Server на 35 человек - Singapore/India/Australia
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1025.00, 0 FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'));
GO

-- Windows Server на 50 человек - Netherlands
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1080.00, 0 FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands'));
GO

-- Windows Server на 50 человек - Germany
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1180.00, 0 FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany'));
GO

-- Windows Server на 50 человек - UK/Poland/France
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1230.00, 0 FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'));
GO

-- Windows Server на 50 человек - Canada/USA
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1180.00, 0 FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa'));
GO

-- Windows Server на 50 человек - Singapore/India/Australia
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 1280.00, 0 FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductVariants WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'));
GO

-- =============================================
-- ШАГ 4: Добавление характеристик товаров (ProductFeatures)
-- =============================================

-- Функция для добавления фичей для Windows Server на 5-8 человек
-- Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12-16 core' FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '45 GB' FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'windows-server-5-8-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-netherlands') AND Name = 'Country');
GO

-- Germany
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12-16 core' FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '45 GB' FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany' FROM Products WHERE Slug = 'windows-server-5-8-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-germany') AND Name = 'Country');
GO

-- UK/Poland/France
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12-16 core' FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '45 GB' FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'United Kingdom / Poland / France' FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-uk-pol-fr') AND Name = 'Country');
GO

-- Canada/USA
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12-16 core' FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '45 GB' FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Canada/ USA' FROM Products WHERE Slug = 'windows-server-5-8-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-canada-usa') AND Name = 'Country');
GO

-- Singapore/India/Australia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '12-16 core' FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '45 GB' FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '150 GB' FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Singapore / India / Australia' FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-5-8-singapore-india-australia') AND Name = 'Country');
GO

-- Windows Server на 15 человек - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '28 core' FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '90 GB' FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '250 GB' FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'windows-server-15-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-netherlands') AND Name = 'Country');
GO

-- Windows Server на 15 человек - Germany
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '28 core' FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '90 GB' FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '250 GB' FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany' FROM Products WHERE Slug = 'windows-server-15-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-germany') AND Name = 'Country');
GO

-- Windows Server на 15 человек - UK/Poland/France
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '28 core' FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '90 GB' FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '250 GB' FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'United Kingdom / Poland / France' FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-uk-pol-fr') AND Name = 'Country');
GO

-- Windows Server на 15 человек - Canada/USA
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '28 core' FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '90 GB' FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '250 GB' FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Canada/ USA' FROM Products WHERE Slug = 'windows-server-15-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-canada-usa') AND Name = 'Country');
GO

-- Windows Server на 15 человек - Singapore/India/Australia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '28 core' FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '90 GB' FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '250 GB' FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Singapore / India / Australia' FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-15-singapore-india-australia') AND Name = 'Country');
GO

-- Windows Server на 25 человек - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '40 core' FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '120 GB' FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '350 GB' FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'windows-server-25-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-netherlands') AND Name = 'Country');
GO

-- Windows Server на 25 человек - Germany
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '40 core' FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '120 GB' FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '350 GB' FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany' FROM Products WHERE Slug = 'windows-server-25-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-germany') AND Name = 'Country');
GO

-- Windows Server на 25 человек - UK/Poland/France
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '40 core' FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '120 GB' FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '350 GB' FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'United Kingdom / Poland / France' FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-uk-pol-fr') AND Name = 'Country');
GO

-- Windows Server на 25 человек - Canada/USA
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '40 core' FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '120 GB' FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '350 GB' FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Canada/ USA' FROM Products WHERE Slug = 'windows-server-25-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-canada-usa') AND Name = 'Country');
GO

-- Windows Server на 25 человек - Singapore/India/Australia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '40 core' FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '120 GB' FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '350 GB' FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Singapore / India / Australia' FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-25-singapore-india-australia') AND Name = 'Country');
GO

-- Windows Server на 35 человек - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '52 core' FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '180 GB' FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '500 GB' FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'windows-server-35-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-netherlands') AND Name = 'Country');
GO

-- Windows Server на 35 человек - Germany
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '52 core' FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '180 GB' FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '500 GB' FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany' FROM Products WHERE Slug = 'windows-server-35-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-germany') AND Name = 'Country');
GO

-- Windows Server на 35 человек - UK/Poland/France
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '52 core' FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '180 GB' FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '500 GB' FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'United Kingdom / Poland / France' FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-uk-pol-fr') AND Name = 'Country');
GO

-- Windows Server на 35 человек - Canada/USA
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '52 core' FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '180 GB' FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '500 GB' FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Canada/ USA' FROM Products WHERE Slug = 'windows-server-35-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-canada-usa') AND Name = 'Country');
GO

-- Windows Server на 35 человек - Singapore/India/Australia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '52 core' FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '180 GB' FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '500 GB' FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Singapore / India / Australia' FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-35-singapore-india-australia') AND Name = 'Country');
GO

-- Windows Server на 50 человек - Netherlands
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '64 core' FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '220 GB' FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '750 GB' FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands' FROM Products WHERE Slug = 'windows-server-50-netherlands'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-netherlands') AND Name = 'Country');
GO

-- Windows Server на 50 человек - Germany
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '64 core' FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '220 GB' FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '750 GB' FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Germany' FROM Products WHERE Slug = 'windows-server-50-germany'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-germany') AND Name = 'Country');
GO

-- Windows Server на 50 человек - UK/Poland/France
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '64 core' FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '220 GB' FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '750 GB' FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'United Kingdom / Poland / France' FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-uk-pol-fr') AND Name = 'Country');
GO

-- Windows Server на 50 человек - Canada/USA
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '64 core' FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '220 GB' FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '750 GB' FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Canada/ USA' FROM Products WHERE Slug = 'windows-server-50-canada-usa'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-canada-usa') AND Name = 'Country');
GO

-- Windows Server на 50 человек - Singapore/India/Australia
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', '64 core' FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia') AND Name = 'CPU');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '220 GB' FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia') AND Name = 'RAM');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSD', '750 GB' FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia') AND Name = 'SSD');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1 GB' FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia') AND Name = 'Network');
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Singapore / India / Australia' FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia'
AND NOT EXISTS (SELECT 1 FROM ProductFeatures WHERE ProductId = (SELECT Id FROM Products WHERE Slug = 'windows-server-50-singapore-india-australia') AND Name = 'Country');
GO

-- =============================================
-- Готово! Все Windows Server товары добавлены в базу данных
-- =============================================

