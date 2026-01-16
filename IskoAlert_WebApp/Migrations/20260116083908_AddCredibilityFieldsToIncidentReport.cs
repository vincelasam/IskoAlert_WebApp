using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IskoAlert_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCredibilityFieldsToIncidentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CredibilityScore",
                table: "IncidentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoProcessed",
                table: "IncidentReports",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AnalysisReason",
                table: "IncidentReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RedFlags",
                table: "IncidentReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositiveSignals",
                table: "IncidentReports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CredibilityScore",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "IsAutoProcessed",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "AnalysisReason",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "RedFlags",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "PositiveSignals",
                table: "IncidentReports");
        }
    }
}