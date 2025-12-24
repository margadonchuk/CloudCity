-- =============================================
-- Скрипт добавления/обновления VPN товаров в базу данных
-- База данных: CloudCityDB
-- =============================================

-- ВАЖНО: Перед выполнением выберите базу данных CloudCityDB!
-- В SQL Server Management Studio выберите CloudCityDB в выпадающем списке
-- Или раскомментируйте следующую строку:
-- USE CloudCityDB;
-- GO

-- =============================================
-- ШАГ 1: Удаление старых VPN товаров (если нужно обновить)
-- =============================================

-- Удаляем старые VPN продукты (можно закомментировать, если хотите сохранить старые)
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 4 AND Slug IN ('vpn-network', 'vpn-device'));
GO

DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Type = 4 AND Slug IN ('vpn-network', 'vpn-device'));
GO

DELETE FROM Products WHERE Type = 4 AND Slug IN ('vpn-network', 'vpn-device');
GO

-- =============================================
-- ШАГ 2: Добавление/обновление VPN товаров
-- =============================================

-- VPN for a network
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPN for a network', 'vpn-network', 4, 'Netherlands/Germany/France', 80.00, 'CHR + Gre tunnel', 1, 1, '/images/vpn-network.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vpn-network' AND Type = 4);
GO

-- VPN for a device
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPN for a device', 'vpn-device', 4, 'Netherlands/Germany/France', 24.00, 'CHR + WireGuard', 1, 1, '/images/vpn-device.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vpn-device' AND Type = 4);
GO

-- VPN for multiple devices
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
SELECT 'VPN for multiple devices', 'vpn-multiple-devices', 4, 'Netherlands/Germany/France', 65.00, 'CHR + WireGuard', 1, 1, '/images/vpn-multiple-devices.png'
WHERE NOT EXISTS (SELECT 1 FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4);
GO

-- =============================================
-- ШАГ 3: Обновление существующих продуктов (если они уже есть)
-- =============================================

UPDATE Products
SET Location = 'Netherlands/Germany/France',
    PricePerMonth = 80.00,
    Configuration = 'CHR + Gre tunnel',
    ImageUrl = '/images/vpn-network.png'
WHERE Slug = 'vpn-network' AND Type = 4;
GO

UPDATE Products
SET Location = 'Netherlands/Germany/France',
    PricePerMonth = 24.00,
    Configuration = 'CHR + WireGuard',
    ImageUrl = '/images/vpn-device.png'
WHERE Slug = 'vpn-device' AND Type = 4;
GO

UPDATE Products
SET Location = 'Netherlands/Germany/France',
    PricePerMonth = 65.00,
    Configuration = 'CHR + WireGuard',
    ImageUrl = '/images/vpn-multiple-devices.png'
WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

-- =============================================
-- ШАГ 4: Добавление вариантов товаров (ProductVariants)
-- =============================================

-- VPN for a network
DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-network' AND Type = 4);
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 80.00, 0 FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

-- VPN for a device
DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-device' AND Type = 4);
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 24.00, 0 FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

-- VPN for multiple devices
DELETE FROM ProductVariants WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4);
GO

INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 65.00, 0 FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

-- =============================================
-- ШАГ 5: Добавление характеристик товаров (ProductFeatures)
-- =============================================

-- VPN for a network - удаляем старые характеристики
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-network' AND Type = 4);
GO

-- VPN for a network - добавляем новые характеристики
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Technology', 'CHR + Gre tunnel' FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Devices', 'Up to 50' FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', 'Unlimited' FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Encryption', 'IPSec' FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vpn-network' AND Type = 4;
GO

-- VPN for a device - удаляем старые характеристики
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-device' AND Type = 4);
GO

-- VPN for a device - добавляем новые характеристики
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Technology', 'CHR + WireGuard' FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Devices', '1 pcs' FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', 'Unlimited' FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Encryption', 'WireGuard' FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vpn-device' AND Type = 4;
GO

-- VPN for multiple devices - удаляем старые характеристики (если есть)
DELETE FROM ProductFeatures WHERE ProductId IN (SELECT Id FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4);
GO

-- VPN for multiple devices - добавляем новые характеристики
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Technology', 'CHR + WireGuard' FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Devices', '5 pcs' FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', 'Unlimited' FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Encryption', 'WireGuard' FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Country', 'Netherlands/Germany/France' FROM Products WHERE Slug = 'vpn-multiple-devices' AND Type = 4;
GO

-- =============================================
-- Готово! Все VPN товары добавлены/обновлены в базе данных
-- =============================================

