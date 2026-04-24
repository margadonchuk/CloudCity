using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    /// <inheritdoc />
    public partial class EnsureBlockedIpsTableExists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var isSqlServer = migrationBuilder.ActiveProvider.Contains("SqlServer", System.StringComparison.OrdinalIgnoreCase);

            if (isSqlServer)
            {
                migrationBuilder.Sql(@"
IF OBJECT_ID(N'[dbo].[BlockedIps]', N'U') IS NULL
BEGIN
    IF OBJECT_ID(N'[dbo].[BlockedIPs]', N'U') IS NOT NULL
        EXEC sp_rename N'[dbo].[BlockedIPs]', N'BlockedIps';
    ELSE IF OBJECT_ID(N'[dbo].[BlockedIP]', N'U') IS NOT NULL
        EXEC sp_rename N'[dbo].[BlockedIP]', N'BlockedIps';
    ELSE IF OBJECT_ID(N'[dbo].[BlockedIp]', N'U') IS NOT NULL
        EXEC sp_rename N'[dbo].[BlockedIp]', N'BlockedIps';
    ELSE IF OBJECT_ID(N'[dbo].[BlockedAddresses]', N'U') IS NOT NULL
        EXEC sp_rename N'[dbo].[BlockedAddresses]', N'BlockedIps';
END;

IF OBJECT_ID(N'[dbo].[BlockedIps]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[BlockedIps] (
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_BlockedIps] PRIMARY KEY,
        [IpAddress] NVARCHAR(45) NOT NULL,
        [Reason] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_BlockedIps_CreatedAt] DEFAULT (GETUTCDATE()),
        [IsActive] BIT NOT NULL CONSTRAINT [DF_BlockedIps_IsActive] DEFAULT ((1))
    );
END;

IF COL_LENGTH(N'dbo.BlockedIps', N'IpAddress') IS NULL
    ALTER TABLE [dbo].[BlockedIps] ADD [IpAddress] NVARCHAR(45) NOT NULL CONSTRAINT [DF_BlockedIps_IpAddress] DEFAULT (N'');

IF COL_LENGTH(N'dbo.BlockedIps', N'Reason') IS NULL
    ALTER TABLE [dbo].[BlockedIps] ADD [Reason] NVARCHAR(500) NULL;

IF COL_LENGTH(N'dbo.BlockedIps', N'CreatedAt') IS NULL
    ALTER TABLE [dbo].[BlockedIps] ADD [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_BlockedIps_CreatedAt2] DEFAULT (GETUTCDATE());

IF COL_LENGTH(N'dbo.BlockedIps', N'IsActive') IS NULL
    ALTER TABLE [dbo].[BlockedIps] ADD [IsActive] BIT NOT NULL CONSTRAINT [DF_BlockedIps_IsActive2] DEFAULT ((1));

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_BlockedIps_IpAddress_IsActive'
      AND object_id = OBJECT_ID(N'[dbo].[BlockedIps]')
)
AND NOT EXISTS (
    SELECT [IpAddress], [IsActive]
    FROM [dbo].[BlockedIps]
    GROUP BY [IpAddress], [IsActive]
    HAVING COUNT(1) > 1
)
BEGIN
    CREATE UNIQUE INDEX [IX_BlockedIps_IpAddress_IsActive] ON [dbo].[BlockedIps]([IpAddress], [IsActive]);
END;
");
            }
            else
            {
                migrationBuilder.Sql(@"
CREATE TABLE IF NOT EXISTS BlockedIps (
    Id INTEGER NOT NULL CONSTRAINT PK_BlockedIps PRIMARY KEY AUTOINCREMENT,
    IpAddress TEXT NOT NULL,
    Reason TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    IsActive INTEGER NOT NULL DEFAULT 1
);

CREATE UNIQUE INDEX IF NOT EXISTS IX_BlockedIps_IpAddress_IsActive ON BlockedIps (IpAddress, IsActive);
");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
