-- Скрипт для удаления всех таблиц в правильном порядке
-- Учитывает внешние ключи

USE CloudCityDB;
GO

-- Отключаем проверку внешних ключей временно
ALTER TABLE ProductFeatures NOCHECK CONSTRAINT ALL;
ALTER TABLE ProductVariants NOCHECK CONSTRAINT ALL;
ALTER TABLE OrderItems NOCHECK CONSTRAINT ALL;
ALTER TABLE Orders NOCHECK CONSTRAINT ALL;
ALTER TABLE AspNetUserClaims NOCHECK CONSTRAINT ALL;
ALTER TABLE AspNetUserLogins NOCHECK CONSTRAINT ALL;
ALTER TABLE AspNetUserRoles NOCHECK CONSTRAINT ALL;
ALTER TABLE AspNetUserTokens NOCHECK CONSTRAINT ALL;
ALTER TABLE AspNetRoleClaims NOCHECK CONSTRAINT ALL;
GO

-- Удаляем таблицы с внешними ключами к Products
IF OBJECT_ID('dbo.ProductFeatures', 'U') IS NOT NULL
    DROP TABLE dbo.ProductFeatures;
GO

IF OBJECT_ID('dbo.ProductVariants', 'U') IS NOT NULL
    DROP TABLE dbo.ProductVariants;
GO

-- Удаляем таблицы с внешними ключами к Orders
IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL
    DROP TABLE dbo.OrderItems;
GO

IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
    DROP TABLE dbo.Orders;
GO

-- Удаляем основные таблицы
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;
GO

IF OBJECT_ID('dbo.Servers', 'U') IS NOT NULL
    DROP TABLE dbo.Servers;
GO

-- Удаляем Identity таблицы (сначала дочерние)
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetUserClaims;
GO

IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetUserLogins;
GO

IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetUserRoles;
GO

IF OBJECT_ID('dbo.AspNetUserTokens', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetUserTokens;
GO

IF OBJECT_ID('dbo.AspNetRoleClaims', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetRoleClaims;
GO

IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetRoles;
GO

IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL
    DROP TABLE dbo.AspNetUsers;
GO

-- Удаляем таблицу миграций
IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL
    DROP TABLE dbo.__EFMigrationsHistory;
GO

PRINT 'Все таблицы удалены успешно';
GO

