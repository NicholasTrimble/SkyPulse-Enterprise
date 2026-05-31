using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkyPulse.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TelemetrySnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeTag = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SatelliteSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProtonSpeed = table.Column<double>(type: "float", nullable: false),
                    ProtonDensity = table.Column<double>(type: "float", nullable: false),
                    ProtonTemperature = table.Column<double>(type: "float", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EnterpriseRiskScore = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetrySnapshots", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelemetrySnapshots");
        }
    }
}
