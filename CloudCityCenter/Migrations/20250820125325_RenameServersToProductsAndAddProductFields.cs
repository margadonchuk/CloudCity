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
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Servers', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Products', N'U') IS NULL
    EXEC sp_rename N'dbo.Servers', N'Products';
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsAvailable') IS NULL
    ALTER TABLE [dbo].[Products] ADD [IsAvailable] bit NOT NULL DEFAULT(0);
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Type') IS NULL
    ALTER TABLE [dbo].[Products] ADD [Type] int NOT NULL DEFAULT(0);
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Configuration') IS NULL
    ALTER TABLE [dbo].[Products] ADD [Configuration] nvarchar(200) NOT NULL DEFAULT('');
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','ImageUrl') IS NULL
    ALTER TABLE [dbo].[Products] ADD [ImageUrl] nvarchar(300) NULL DEFAULT(NULL);
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsPublished') IS NULL
    ALTER TABLE [dbo].[Products] ADD [IsPublished] bit NOT NULL DEFAULT(0);
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Location') IS NULL
    ALTER TABLE [dbo].[Products] ADD [Location] nvarchar(100) NOT NULL DEFAULT('');
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','PricePerMonth') IS NULL
    ALTER TABLE [dbo].[Products] ADD [PricePerMonth] decimal(18,2) NOT NULL DEFAULT(0);
");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','Slug') IS NULL
    ALTER TABLE [dbo].[Products] ADD [Slug] nvarchar(100) NOT NULL DEFAULT('');
");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Slug",
                table: "Products",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Slug",
                table: "Products");

            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Products','IsAvailable') IS NOT NULL
    ALTER TABLE [dbo].[Products] DROP COLUMN [IsAvailable];
");

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

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL AND OBJECT_ID(N'dbo.Servers', N'U') IS NULL
    EXEC sp_rename N'dbo.Products', N'Servers';
");
        }
    }
}
