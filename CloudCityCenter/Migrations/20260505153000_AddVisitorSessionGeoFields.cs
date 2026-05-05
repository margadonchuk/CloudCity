using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    public partial class AddVisitorSessionGeoFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "VisitorSessions",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "VisitorSessions",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "VisitorSessions",
                type: "TEXT",
                maxLength: 2,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "VisitorSessions");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "VisitorSessions");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "VisitorSessions");
        }
    }
}
