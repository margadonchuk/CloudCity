using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedIpAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider.Contains("SqlServer"))
            {
                migrationBuilder.Sql(
                    """
                    IF OBJECT_ID(N'[dbo].[BlockedIpAddresses]', N'U') IS NULL
                    BEGIN
                        CREATE TABLE [BlockedIpAddresses] (
                            [Id] int NOT NULL IDENTITY,
                            [IpAddress] nvarchar(45) NOT NULL,
                            [NormalizedIpAddress] nvarchar(45) NOT NULL,
                            [Reason] nvarchar(500) NULL,
                            [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
                            [CreatedBy] nvarchar(256) NULL,
                            [IsActive] bit NOT NULL DEFAULT CAST(1 AS bit),
                            CONSTRAINT [PK_BlockedIpAddresses] PRIMARY KEY ([Id])
                        );
                    END
                    """
                );

                migrationBuilder.Sql(
                    """
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.indexes
                        WHERE name = 'IX_BlockedIpAddresses_NormalizedIpAddress_IsActive'
                          AND object_id = OBJECT_ID('BlockedIpAddresses')
                    )
                    CREATE UNIQUE INDEX [IX_BlockedIpAddresses_NormalizedIpAddress_IsActive]
                        ON [BlockedIpAddresses] ([NormalizedIpAddress], [IsActive]);
                    """
                );

                return;
            }

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "BlockedIpAddresses" (
                    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
                    "IpAddress" TEXT NOT NULL,
                    "NormalizedIpAddress" TEXT NOT NULL,
                    "Reason" TEXT NULL,
                    "CreatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    "CreatedBy" TEXT NULL,
                    "IsActive" INTEGER NOT NULL DEFAULT 1
                );
                """
            );

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_BlockedIpAddresses_NormalizedIpAddress_IsActive"
                    ON "BlockedIpAddresses" ("NormalizedIpAddress", "IsActive");
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider.Contains("SqlServer"))
            {
                migrationBuilder.Sql(
                    """
                    IF OBJECT_ID(N'[dbo].[BlockedIpAddresses]', N'U') IS NOT NULL
                        DROP TABLE [BlockedIpAddresses];
                    """
                );

                return;
            }

            migrationBuilder.Sql("DROP TABLE IF EXISTS \"BlockedIpAddresses\";");
        }
    }
}
