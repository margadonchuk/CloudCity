using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    /// <inheritdoc />
    public partial class RenameServersToProductsAndAddProductFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Servers', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    EXEC sp_rename N'dbo.Servers', N'Products';
END");

                migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Servers_Slug' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    EXEC sp_rename N'dbo.Products.IX_Servers_Slug', N'IX_Products_Slug', N'INDEX';
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsAvailable') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD IsAvailable bit NOT NULL DEFAULT(0);
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Type') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD Type int NOT NULL DEFAULT(0);
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Configuration') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD Configuration nvarchar(200) NOT NULL DEFAULT('');
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','ImageUrl') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD ImageUrl nvarchar(300) NULL;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsPublished') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD IsPublished bit NOT NULL DEFAULT(0);
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Location') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD Location nvarchar(100) NOT NULL DEFAULT('');
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','PricePerMonth') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD PricePerMonth decimal(18,2) NOT NULL DEFAULT(0);
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Slug') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD Slug nvarchar(100) NOT NULL DEFAULT('');
END");
            }
            else
            {
                migrationBuilder.Sql("ALTER TABLE Servers RENAME TO Products");

                migrationBuilder.AddColumn<bool>(
                    name: "IsAvailable",
                    table: "Products",
                    nullable: false,
                    defaultValue: false);

                migrationBuilder.AddColumn<int>(
                    name: "Type",
                    table: "Products",
                    nullable: false,
                    defaultValue: 0);

                migrationBuilder.AddColumn<string>(
                    name: "Configuration",
                    table: "Products",
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false,
                    defaultValue: "");

                migrationBuilder.AddColumn<string>(
                    name: "ImageUrl",
                    table: "Products",
                    type: "nvarchar(300)",
                    maxLength: 300,
                    nullable: true);

                migrationBuilder.AddColumn<bool>(
                    name: "IsPublished",
                    table: "Products",
                    nullable: false,
                    defaultValue: false);

                migrationBuilder.AddColumn<string>(
                    name: "Location",
                    table: "Products",
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false,
                    defaultValue: "");

                migrationBuilder.AddColumn<decimal>(
                    name: "PricePerMonth",
                    table: "Products",
                    type: "decimal(18,2)",
                    nullable: false,
                    defaultValue: 0m);

                migrationBuilder.AddColumn<string>(
                    name: "Slug",
                    table: "Products",
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: false,
                    defaultValue: "");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsAvailable') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN IsAvailable;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Type') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN Type;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Configuration') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN Configuration;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','ImageUrl') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN ImageUrl;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsPublished') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN IsPublished;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Location') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN Location;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','PricePerMonth') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN PricePerMonth;
END");

                migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Slug') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Products DROP COLUMN Slug;
END");

                migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Slug' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    EXEC sp_rename N'dbo.Products.IX_Products_Slug', N'IX_Servers_Slug', N'INDEX';
END");

                migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Servers', N'U') IS NULL
BEGIN
    EXEC sp_rename N'dbo.Products', N'Servers';
END");
            }
            else
            {
                migrationBuilder.DropColumn(
                    name: "IsAvailable",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "Type",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "Configuration",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "ImageUrl",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "IsPublished",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "Location",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "PricePerMonth",
                    table: "Products");

                migrationBuilder.DropColumn(
                    name: "Slug",
                    table: "Products");

                migrationBuilder.Sql("ALTER TABLE Products RENAME TO Servers");
            }
        }
    }
}
