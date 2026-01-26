using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessageIpAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider.Contains("SqlServer"))
            {
                migrationBuilder.Sql(
                    """
                    IF OBJECT_ID(N'[dbo].[ContactMessages]', N'U') IS NULL
                    BEGIN
                        CREATE TABLE [ContactMessages] (
                            [Id] int NOT NULL IDENTITY,
                            [Name] nvarchar(200) NOT NULL,
                            [Email] nvarchar(200) NOT NULL,
                            [Phone] nvarchar(50) NULL,
                            [Subject] nvarchar(200) NULL,
                            [ServiceType] nvarchar(100) NULL,
                            [IpAddress] nvarchar(45) NULL,
                            [Message] nvarchar(max) NOT NULL,
                            [SourcePage] nvarchar(50) NOT NULL,
                            [CreatedAt] datetime2 NOT NULL,
                            [IsRead] bit NOT NULL,
                            [ReadAt] datetime2 NULL,
                            CONSTRAINT [PK_ContactMessages] PRIMARY KEY ([Id])
                        );
                    END
                    """
                );

                migrationBuilder.Sql(
                    """
                    IF COL_LENGTH('ContactMessages', 'IpAddress') IS NULL
                        ALTER TABLE [ContactMessages] ADD [IpAddress] nvarchar(45) NULL;
                    """
                );

                return;
            }

            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS "ContactMessages" (
                    "Id" INTEGER PRIMARY KEY AUTOINCREMENT,
                    "Name" TEXT NOT NULL,
                    "Email" TEXT NOT NULL,
                    "Phone" TEXT NULL,
                    "Subject" TEXT NULL,
                    "ServiceType" TEXT NULL,
                    "IpAddress" TEXT NULL,
                    "Message" TEXT NOT NULL,
                    "SourcePage" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "IsRead" INTEGER NOT NULL,
                    "ReadAt" TEXT NULL
                );
                """
            );

            migrationBuilder.Sql(
                """
                ALTER TABLE "ContactMessages" ADD COLUMN IF NOT EXISTS "IpAddress" TEXT;
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
                    IF COL_LENGTH('ContactMessages', 'IpAddress') IS NOT NULL
                        ALTER TABLE [ContactMessages] DROP COLUMN [IpAddress];
                    """
                );
            }
        }
    }
}
