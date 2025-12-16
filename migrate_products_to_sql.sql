-- Скрипт миграции товаров и услуг в SQL Server
-- База данных: CloudCityDB
-- Сервер: 10.151.10.8

USE CloudCityDB;
GO

-- Очистка существующих данных (опционально, раскомментируйте если нужно)
-- DELETE FROM ProductFeatures;
-- DELETE FROM ProductVariants;
-- DELETE FROM Products;
-- GO

-- Вставка товаров и услуг
-- Dedicated Servers
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
VALUES 
    ('DS1-E3', 'ds1-e3', 0, 'Germany', 59.00, 'Xeon E3-1270v6, 32GB RAM, 2x240GB SSD', 1, 1, '/images/dell.png'),
    ('DS2-E5', 'ds2-e5', 0, 'Netherlands', 79.00, 'Xeon E5-1650v4, 64GB RAM, 2x480GB SSD', 1, 1, '/images/hp.png'),
    ('DS3-Ryzen', 'ds3-ryzen', 0, 'France', 95.00, 'Ryzen 5 3600, 64GB RAM, 2x1TB NVMe', 1, 1, '/images/ovh.png'),
    ('DS4-EPYC', 'ds4-epyc', 0, 'Finland', 129.00, 'AMD EPYC 7501, 128GB RAM, 2x1.92TB NVMe', 1, 1, '/images/hetzner.png'),
    ('DS5-Storage', 'ds5-storage', 0, 'USA', 149.00, 'Xeon Silver 4210, 64GB RAM, 4x4TB HDD', 1, 1, '/images/proxmox.png'),
    ('DS6-HighMem', 'ds6-highmem', 0, 'Singapore', 169.00, 'Xeon Gold 6130, 256GB RAM, 2x960GB SSD', 1, 1, '/images/ms.png');
GO

-- VPS
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
VALUES 
    ('VPS1-1-20', 'vps1-1-20', 3, 'Germany', 10.00, 'Intel 1 Core, 1GB RAM, 20GB SSD', 1, 1, '/images/vps1.png'),
    ('VPS2-2-20', 'vps2-2-20', 3, 'Germany', 12.00, 'Intel 2 Cores, 2GB RAM, 20GB SSD', 1, 1, '/images/vps2.png'),
    ('VPS3-4-40', 'vps3-4-40', 3, 'Germany', 19.00, 'AMD | Intel 4 Cores, 4GB RAM, 40GB SSD', 1, 1, '/images/vps3.png'),
    ('VPS4-4-80', 'vps4-4-80', 3, 'Germany', 24.00, 'AMD | Intel 4 Cores, 4GB RAM, 80GB SSD', 1, 1, '/images/vps4.png'),
    ('VPS5-4-80', 'vps5-4-80', 3, 'Germany', 28.00, 'AMD | Intel 4 Cores, 4GB RAM, 80GB SSD', 1, 1, '/images/vps5.png'),
    ('VPS6-6-100', 'vps6-6-100', 3, 'Germany', 35.00, 'AMD | Intel 4 Cores, 6GB RAM, 100GB SSD', 1, 1, '/images/vps6.png');
GO

-- VPN Services
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
VALUES 
    ('VPN for a network', 'vpn-network', 4, 'Global', 80.00, 'Site-to-site VPN service', 1, 1, '/images/vpn-network.png'),
    ('VPN for a device', 'vpn-device', 4, 'Global', 25.00, 'Single device VPN', 1, 1, '/images/vpn-device.png');
GO

-- Hosting Services
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
VALUES 
    ('Basic Hosting', 'basic-hosting', 1, 'US', 10.00, 'Shared hosting', 1, 1, 'https://via.placeholder.com/300x200?text=Hosting');
GO

-- Website Services
INSERT INTO Products (Name, Slug, Type, Location, PricePerMonth, Configuration, IsAvailable, IsPublished, ImageUrl)
VALUES 
    ('Starter Website', 'starter-website', 2, 'US', 20.00, 'WordPress site', 1, 1, 'https://via.placeholder.com/300x200?text=Website');
GO

-- Вставка вариантов товаров (ProductVariants)
-- DS1-E3
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 59.00, 0 FROM Products WHERE Slug = 'ds1-e3';
GO

-- DS2-E5
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 79.00, 0 FROM Products WHERE Slug = 'ds2-e5';
GO

-- DS3-Ryzen
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 95.00, 0 FROM Products WHERE Slug = 'ds3-ryzen';
GO

-- DS4-EPYC
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 129.00, 0 FROM Products WHERE Slug = 'ds4-epyc';
GO

-- DS5-Storage
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 149.00, 0 FROM Products WHERE Slug = 'ds5-storage';
GO

-- DS6-HighMem
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 169.00, 0 FROM Products WHERE Slug = 'ds6-highmem';
GO

-- VPS1-1-20
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 10.00, 0 FROM Products WHERE Slug = 'vps1-1-20';
GO

-- VPS2-2-20
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 12.00, 0 FROM Products WHERE Slug = 'vps2-2-20';
GO

-- VPS3-4-40
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 19.00, 0 FROM Products WHERE Slug = 'vps3-4-40';
GO

-- VPS4-4-80
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 24.00, 0 FROM Products WHERE Slug = 'vps4-4-80';
GO

-- VPS5-4-80
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 28.00, 0 FROM Products WHERE Slug = 'vps5-4-80';
GO

-- VPS6-6-100
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 35.00, 0 FROM Products WHERE Slug = 'vps6-6-100';
GO

-- VPN for a network
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 80.00, 0 FROM Products WHERE Slug = 'vpn-network';
GO

-- VPN for a device
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 25.00, 0 FROM Products WHERE Slug = 'vpn-device';
GO

-- Basic Hosting
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 10.00, 0 FROM Products WHERE Slug = 'basic-hosting';
GO

-- Starter Website
INSERT INTO ProductVariants (ProductId, Name, Price, BillingPeriod)
SELECT Id, 'Monthly', 20.00, 0 FROM Products WHERE Slug = 'starter-website';
GO

-- Вставка характеристик товаров (ProductFeatures)
-- DS1-E3 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Xeon E3-1270v6' FROM Products WHERE Slug = 'ds1-e3';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '32GB' FROM Products WHERE Slug = 'ds1-e3';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '2x240GB SSD' FROM Products WHERE Slug = 'ds1-e3';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1Gbps' FROM Products WHERE Slug = 'ds1-e3';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds1-e3';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany' FROM Products WHERE Slug = 'ds1-e3';
GO

-- DS2-E5 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Xeon E5-1650v4' FROM Products WHERE Slug = 'ds2-e5';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '64GB' FROM Products WHERE Slug = 'ds2-e5';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '2x480GB SSD' FROM Products WHERE Slug = 'ds2-e5';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1Gbps' FROM Products WHERE Slug = 'ds2-e5';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds2-e5';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Netherlands' FROM Products WHERE Slug = 'ds2-e5';
GO

-- DS3-Ryzen Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Ryzen 5 3600' FROM Products WHERE Slug = 'ds3-ryzen';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '64GB' FROM Products WHERE Slug = 'ds3-ryzen';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '2x1TB NVMe' FROM Products WHERE Slug = 'ds3-ryzen';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1Gbps' FROM Products WHERE Slug = 'ds3-ryzen';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds3-ryzen';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'France' FROM Products WHERE Slug = 'ds3-ryzen';
GO

-- DS4-EPYC Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'AMD EPYC 7501' FROM Products WHERE Slug = 'ds4-epyc';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '128GB' FROM Products WHERE Slug = 'ds4-epyc';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '2x1.92TB NVMe' FROM Products WHERE Slug = 'ds4-epyc';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '2Gbps' FROM Products WHERE Slug = 'ds4-epyc';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds4-epyc';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Finland' FROM Products WHERE Slug = 'ds4-epyc';
GO

-- DS5-Storage Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Xeon Silver 4210' FROM Products WHERE Slug = 'ds5-storage';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '64GB' FROM Products WHERE Slug = 'ds5-storage';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '4x4TB HDD' FROM Products WHERE Slug = 'ds5-storage';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1Gbps' FROM Products WHERE Slug = 'ds5-storage';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds5-storage';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'USA' FROM Products WHERE Slug = 'ds5-storage';
GO

-- DS6-HighMem Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Xeon Gold 6130' FROM Products WHERE Slug = 'ds6-highmem';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '256GB' FROM Products WHERE Slug = 'ds6-highmem';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '2x960GB SSD' FROM Products WHERE Slug = 'ds6-highmem';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '2Gbps' FROM Products WHERE Slug = 'ds6-highmem';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'ds6-highmem';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Singapore' FROM Products WHERE Slug = 'ds6-highmem';
GO

-- VPS1-1-20 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Intel 1 Core' FROM Products WHERE Slug = 'vps1-1-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '1GB' FROM Products WHERE Slug = 'vps1-1-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '20GB SSD' FROM Products WHERE Slug = 'vps1-1-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '100Mbps Bandwidth' FROM Products WHERE Slug = 'vps1-1-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps1-1-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France' FROM Products WHERE Slug = 'vps1-1-20';
GO

-- VPS2-2-20 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'Intel 2 Cores' FROM Products WHERE Slug = 'vps2-2-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '2GB' FROM Products WHERE Slug = 'vps2-2-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '20GB SSD' FROM Products WHERE Slug = 'vps2-2-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '250Mbps Bandwidth' FROM Products WHERE Slug = 'vps2-2-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps2-2-20';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France' FROM Products WHERE Slug = 'vps2-2-20';
GO

-- VPS3-4-40 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'AMD | Intel 4 Cores' FROM Products WHERE Slug = 'vps3-4-40';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '4GB' FROM Products WHERE Slug = 'vps3-4-40';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '40GB SSD' FROM Products WHERE Slug = 'vps3-4-40';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '250Mbps Bandwidth' FROM Products WHERE Slug = 'vps3-4-40';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps3-4-40';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France | Netherlands' FROM Products WHERE Slug = 'vps3-4-40';
GO

-- VPS4-4-80 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'AMD | Intel 4 Cores' FROM Products WHERE Slug = 'vps4-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '4GB' FROM Products WHERE Slug = 'vps4-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '80GB SSD' FROM Products WHERE Slug = 'vps4-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '500Mbps Bandwidth' FROM Products WHERE Slug = 'vps4-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps4-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France | Netherlands | Singapore | USA' FROM Products WHERE Slug = 'vps4-4-80';
GO

-- VPS5-4-80 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'AMD | Intel 4 Cores' FROM Products WHERE Slug = 'vps5-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '4GB' FROM Products WHERE Slug = 'vps5-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '80GB SSD' FROM Products WHERE Slug = 'vps5-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1000Mbps Bandwidth' FROM Products WHERE Slug = 'vps5-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps5-4-80';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France | Netherlands | Singapore | USA' FROM Products WHERE Slug = 'vps5-4-80';
GO

-- VPS6-6-100 Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'CPU', 'AMD | Intel 4 Cores' FROM Products WHERE Slug = 'vps6-6-100';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'RAM', '6GB' FROM Products WHERE Slug = 'vps6-6-100';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '100GB SSD' FROM Products WHERE Slug = 'vps6-6-100';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Network', '1000Mbps Bandwidth' FROM Products WHERE Slug = 'vps6-6-100';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'IPv4', '1' FROM Products WHERE Slug = 'vps6-6-100';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Location', 'Germany | Poland | France | Netherlands | Singapore | USA' FROM Products WHERE Slug = 'vps6-6-100';
GO

-- VPN for a network Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Devices', 'Up to 50' FROM Products WHERE Slug = 'vpn-network';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', 'Unlimited' FROM Products WHERE Slug = 'vpn-network';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Support', '24/7' FROM Products WHERE Slug = 'vpn-network';
GO

-- VPN for a device Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Traffic', 'Unlimited' FROM Products WHERE Slug = 'vpn-device';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Servers', '50+ Countries' FROM Products WHERE Slug = 'vpn-device';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Encryption', 'AES-256' FROM Products WHERE Slug = 'vpn-device';
GO

-- Basic Hosting Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Storage', '10 GB' FROM Products WHERE Slug = 'basic-hosting';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Bandwidth', '100 GB' FROM Products WHERE Slug = 'basic-hosting';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Email Accounts', '5' FROM Products WHERE Slug = 'basic-hosting';
GO

-- Starter Website Features
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Pages', '5' FROM Products WHERE Slug = 'starter-website';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'Support', 'Email Support' FROM Products WHERE Slug = 'starter-website';
INSERT INTO ProductFeatures (ProductId, Name, Value)
SELECT Id, 'SSL', 'Included' FROM Products WHERE Slug = 'starter-website';
GO

PRINT 'Миграция товаров и услуг завершена успешно!';
GO

