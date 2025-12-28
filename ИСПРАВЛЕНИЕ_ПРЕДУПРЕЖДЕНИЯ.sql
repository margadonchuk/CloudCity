-- Скрипт для исправления предупреждения о длине ключа PK_AspNetUserRoles
-- Выполните этот скрипт, если хотите убрать предупреждение

USE CloudCityDB;
GO

-- Удаляем существующий первичный ключ
ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [PK_AspNetUserRoles];
GO

-- Создаем новый первичный ключ как некластеризованный
ALTER TABLE [AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY NONCLUSTERED ([UserId], [RoleId]);
GO

-- Создаем кластеризованный индекс на UserId для производительности
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUserRoles_UserId' AND object_id = OBJECT_ID('AspNetUserRoles'))
    CREATE CLUSTERED INDEX [IX_AspNetUserRoles_UserId] ON [AspNetUserRoles] ([UserId]);
GO

PRINT 'Предупреждение исправлено. Таблица AspNetUserRoles оптимизирована.';
GO















