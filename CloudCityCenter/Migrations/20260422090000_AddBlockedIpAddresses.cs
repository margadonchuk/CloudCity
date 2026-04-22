using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockedIps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var isSqlServer = migrationBuilder.ActiveProvider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

            if (isSqlServer)
            {
                migrationBuilder.CreateTable(
                    name: "BlockedIps",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        IpAddress = table.Column<string>(maxLength: 45, nullable: false),
                        Reason = table.Column<string>(maxLength: 500, nullable: true),
                        CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                        IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_BlockedIps", x => x.Id);
                    });
            }
            else
            {
                migrationBuilder.CreateTable(
                    name: "BlockedIps",
                    columns: table => new
                    {
                        Id = table.Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        IpAddress = table.Column<string>(maxLength: 45, nullable: false),
                        Reason = table.Column<string>(maxLength: 500, nullable: true),
                        CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                        IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_BlockedIps", x => x.Id);
                    });
            }

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIps_IpAddress_IsActive",
                table: "BlockedIps",
                columns: new[] { "IpAddress", "IsActive" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedIps");
        }
    }
}
