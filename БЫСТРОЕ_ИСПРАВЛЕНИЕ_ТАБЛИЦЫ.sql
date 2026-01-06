-- Быстрое исправление таблицы ContactMessages
-- Выполните этот скрипт в SQL Server Management Studio

USE CloudCityDB;
GO

-- Проверка существования таблицы
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ContactMessages]') AND type in (N'U'))
BEGIN
    PRINT 'Таблица ContactMessages существует. Проверяем структуру...';
    
    -- Проверка структуры
    SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        CHARACTER_MAXIMUM_LENGTH,
        IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'ContactMessages'
    ORDER BY ORDINAL_POSITION;
    
    -- Если нужно пересоздать (ВНИМАНИЕ: удалит данные!)
    -- Раскомментируйте следующие строки:
    /*
    DROP TABLE [dbo].[ContactMessages];
    GO
    
    CREATE TABLE [dbo].[ContactMessages] (
        [Id] INT NOT NULL IDENTITY(1,1),
        [Name] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [Phone] NVARCHAR(50) NULL,
        [Subject] NVARCHAR(200) NULL,
        [ServiceType] NVARCHAR(100) NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [SourcePage] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsRead] BIT NOT NULL DEFAULT 0,
        [ReadAt] DATETIME2 NULL,
        CONSTRAINT [PK_ContactMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    GO
    
    PRINT 'Таблица ContactMessages пересоздана';
    */
END
ELSE
BEGIN
    PRINT 'Таблица ContactMessages не существует. Создаем...';
    
    CREATE TABLE [dbo].[ContactMessages] (
        [Id] INT NOT NULL IDENTITY(1,1),
        [Name] NVARCHAR(200) NOT NULL,
        [Email] NVARCHAR(200) NOT NULL,
        [Phone] NVARCHAR(50) NULL,
        [Subject] NVARCHAR(200) NULL,
        [ServiceType] NVARCHAR(100) NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [SourcePage] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsRead] BIT NOT NULL DEFAULT 0,
        [ReadAt] DATETIME2 NULL,
        CONSTRAINT [PK_ContactMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Таблица ContactMessages создана';
END
GO

-- Проверка первичного ключа
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'PK_ContactMessages' AND object_id = OBJECT_ID('dbo.ContactMessages'))
BEGIN
    PRINT 'Добавляем первичный ключ...';
    ALTER TABLE [dbo].[ContactMessages]
    ADD CONSTRAINT [PK_ContactMessages] PRIMARY KEY CLUSTERED ([Id] ASC);
    PRINT 'Первичный ключ добавлен';
END
ELSE
BEGIN
    PRINT 'Первичный ключ уже существует';
END
GO

PRINT 'Проверка завершена';
GO

