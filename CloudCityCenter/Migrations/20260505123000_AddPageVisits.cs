using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudCityCenter.Migrations
{
    public partial class AddPageVisits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitorSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                    QueryString = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    HttpMethod = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    VisitedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageVisits_VisitorSessions_VisitorSessionId",
                        column: x => x.VisitorSessionId,
                        principalTable: "VisitorSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageVisits_VisitedAt",
                table: "PageVisits",
                column: "VisitedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PageVisits_VisitorSessionId",
                table: "PageVisits",
                column: "VisitorSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageVisits");
        }
    }
}
