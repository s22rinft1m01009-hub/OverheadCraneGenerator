using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraneGenerator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CraneDesigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DesignName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CraneType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CapacityTonnes = table.Column<double>(type: "REAL", nullable: false),
                    SpanMm = table.Column<double>(type: "REAL", nullable: false),
                    LiftHeightMm = table.Column<double>(type: "REAL", nullable: false),
                    RunwayWidthMm = table.Column<double>(type: "REAL", nullable: false),
                    HoistPositionMm = table.Column<double>(type: "REAL", nullable: false),
                    BuildingHeightMm = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraneDesigns", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraneDesigns");
        }
    }
}
