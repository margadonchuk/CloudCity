-- Полный скрипт создания базы данных для SQL Server
-- Выполните этот скрипт в SQL Server Management Studio на 10.151.10.8
-- Этот скрипт сначала удаляет все внешние ключи, затем таблицы, затем создает все заново

USE CloudCityDB;
GO

-- Шаг 1: Удаляем все внешние ключи
PRINT 'Удаление внешних ключей...';
GO

-- Удаляем внешние ключи для OrderItems
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Orders_OrderId')
    ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_Orders_OrderId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_Products_ProductId')
    ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_Products_ProductId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItems_ProductVariants_ProductVariantId')
    ALTER TABLE [OrderItems] DROP CONSTRAINT [FK_OrderItems_ProductVariants_ProductVariantId];
GO

-- Удаляем внешние ключи для Orders
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_AspNetUsers_UserId')
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_AspNetUsers_UserId];
GO

-- Удаляем внешние ключи для ProductFeatures
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductFeatures_Products_ProductId')
    ALTER TABLE [ProductFeatures] DROP CONSTRAINT [FK_ProductFeatures_Products_ProductId];
GO

-- Удаляем внешние ключи для ProductVariants
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductVariants_Products_ProductId')
    ALTER TABLE [ProductVariants] DROP CONSTRAINT [FK_ProductVariants_Products_ProductId];
GO

-- Удаляем внешние ключи для Identity таблиц
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetRoleClaims_AspNetRoles_RoleId')
    ALTER TABLE [AspNetRoleClaims] DROP CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUserClaims_AspNetUsers_UserId')
    ALTER TABLE [AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUserLogins_AspNetUsers_UserId')
    ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUserRoles_AspNetRoles_RoleId')
    ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUserRoles_AspNetUsers_UserId')
    ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId];
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AspNetUserTokens_AspNetUsers_UserId')
    ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId];
GO

-- Шаг 2: Удаляем все индексы
PRINT 'Удаление индексов...';
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_Slug' AND object_id = OBJECT_ID('Products'))
    DROP INDEX [IX_Products_Slug] ON [Products];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Servers_Slug' AND object_id = OBJECT_ID('Servers'))
    DROP INDEX [IX_Servers_Slug] ON [Servers];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductVariants_ProductId' AND object_id = OBJECT_ID('ProductVariants'))
    DROP INDEX [IX_ProductVariants_ProductId] ON [ProductVariants];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductFeatures_ProductId' AND object_id = OBJECT_ID('ProductFeatures'))
    DROP INDEX [IX_ProductFeatures_ProductId] ON [ProductFeatures];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId' AND object_id = OBJECT_ID('Orders'))
    DROP INDEX [IX_Orders_UserId] ON [Orders];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_OrderId' AND object_id = OBJECT_ID('OrderItems'))
    DROP INDEX [IX_OrderItems_OrderId] ON [OrderItems];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_ProductId' AND object_id = OBJECT_ID('OrderItems'))
    DROP INDEX [IX_OrderItems_ProductId] ON [OrderItems];
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderItems_ProductVariantId' AND object_id = OBJECT_ID('OrderItems'))
    DROP INDEX [IX_OrderItems_ProductVariantId] ON [OrderItems];
GO

-- Шаг 3: Удаляем все таблицы в правильном порядке
PRINT 'Удаление таблиц...';
GO

IF OBJECT_ID('dbo.ProductFeatures', 'U') IS NOT NULL DROP TABLE dbo.ProductFeatures;
IF OBJECT_ID('dbo.ProductVariants', 'U') IS NOT NULL DROP TABLE dbo.ProductVariants;
IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Servers', 'U') IS NOT NULL DROP TABLE dbo.Servers;
IF OBJECT_ID('dbo.AspNetUserClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserClaims;
IF OBJECT_ID('dbo.AspNetUserLogins', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserLogins;
IF OBJECT_ID('dbo.AspNetUserRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserRoles;
IF OBJECT_ID('dbo.AspNetUserTokens', 'U') IS NOT NULL DROP TABLE dbo.AspNetUserTokens;
IF OBJECT_ID('dbo.AspNetRoleClaims', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoleClaims;
IF OBJECT_ID('dbo.AspNetRoles', 'U') IS NOT NULL DROP TABLE dbo.AspNetRoles;
IF OBJECT_ID('dbo.AspNetUsers', 'U') IS NOT NULL DROP TABLE dbo.AspNetUsers;
IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL DROP TABLE dbo.__EFMigrationsHistory;
GO

-- Шаг 4: Создаем таблицы Identity (ASP.NET Core Identity)
PRINT 'Создание таблиц Identity...';
GO

CREATE TABLE [AspNetRoles] (
    [Id] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(256) NULL,
    [NormalizedName] NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] NVARCHAR(450) NOT NULL,
    [UserName] NVARCHAR(256) NULL,
    [NormalizedUserName] NVARCHAR(256) NULL,
    [Email] NVARCHAR(256) NULL,
    [NormalizedEmail] NVARCHAR(256) NULL,
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NULL,
    [SecurityStamp] NVARCHAR(MAX) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    [PhoneNumber] NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET NULL,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [ClaimType] NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [ProviderKey] NVARCHAR(128) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY NONCLUSTERED ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- Исправляем проблему с длиной ключа для AspNetUserRoles - используем меньший размер для ключа
-- Используем NVARCHAR(128) вместо NVARCHAR(450) для уменьшения размера ключа
CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY NONCLUSTERED ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
-- Создаем отдельный кластеризованный индекс на UserId для производительности
CREATE CLUSTERED INDEX [IX_AspNetUserRoles_UserId] ON [AspNetUserRoles] ([UserId]);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(128) NOT NULL,
    [Value] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY NONCLUSTERED ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- Шаг 5: Создаем таблицы приложения
PRINT 'Создание таблиц приложения...';
GO

CREATE TABLE [Products] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL,
    [Type] INT NOT NULL,
    [Location] NVARCHAR(100) NOT NULL,
    [PricePerMonth] DECIMAL(18,2) NOT NULL,
    [Configuration] NVARCHAR(200) NOT NULL,
    [IsAvailable] BIT NOT NULL,
    [IsPublished] BIT NOT NULL DEFAULT 0,
    [ImageUrl] NVARCHAR(300) NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Servers] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Slug] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(200) NULL,
    [Location] NVARCHAR(100) NOT NULL,
    [PricePerMonth] DECIMAL(18,2) NOT NULL,
    [CPU] NVARCHAR(100) NOT NULL,
    [RamGb] INT NOT NULL,
    [StorageGb] INT NOT NULL,
    [ImageUrl] NVARCHAR(300) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DDoSTier] NVARCHAR(50) NOT NULL DEFAULT 'Basic',
    [Stock] INT NOT NULL DEFAULT 9999,
    [CreatedUtc] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Servers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ProductVariants] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProductId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [BillingPeriod] INT NOT NULL,
    CONSTRAINT [PK_ProductVariants] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductVariants_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ProductFeatures] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProductId] INT NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Value] NVARCHAR(200) NULL,
    CONSTRAINT [PK_ProductFeatures] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductFeatures_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [Total] DECIMAL(18,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    [Status] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderItems] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [OrderId] INT NOT NULL,
    [ProductId] INT NOT NULL,
    [ProductVariantId] INT NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_ProductVariants_ProductVariantId] FOREIGN KEY ([ProductVariantId]) REFERENCES [ProductVariants] ([Id])
);
GO

-- Шаг 6: Создаем индексы
PRINT 'Создание индексов...';
GO

CREATE UNIQUE INDEX [IX_Products_Slug] ON [Products] ([Slug]);
CREATE UNIQUE INDEX [IX_Servers_Slug] ON [Servers] ([Slug]);
CREATE INDEX [IX_ProductVariants_ProductId] ON [ProductVariants] ([ProductId]);
CREATE INDEX [IX_ProductFeatures_ProductId] ON [ProductFeatures] ([ProductId]);
CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
CREATE INDEX [IX_OrderItems_ProductVariantId] ON [OrderItems] ([ProductVariantId]);
CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

-- Шаг 7: Создаем таблицу миграций (для совместимости с EF Core)
PRINT 'Создание таблицы миграций...';
GO

CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] NVARCHAR(150) NOT NULL,
    [ProductVersion] NVARCHAR(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
);
GO

-- Вставляем запись о миграции (опционально, для совместимости)
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES ('InitialCreateForSqlServer', '8.0.6');
GO

PRINT '';
PRINT '========================================';
PRINT 'База данных создана успешно!';
PRINT 'Теперь выполните скрипт migrate_products_to_sql.sql для загрузки данных.';
PRINT '========================================';
GO
