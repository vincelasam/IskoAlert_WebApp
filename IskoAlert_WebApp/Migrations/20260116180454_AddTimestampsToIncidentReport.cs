using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IskoAlert_WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampsToIncidentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoProcessed",
                table: "IncidentReports",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "CredibilityScore",
                table: "IncidentReports",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InProgressAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResolvedAt",
                table: "IncidentReports",
                type: "datetime2",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LostFoundItems");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "InProgressAt",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "IncidentType",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "IncidentReports");

            migrationBuilder.DropColumn(
                name: "ResolvedAt",
                table: "IncidentReports");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAutoProcessed",
                table: "IncidentReports",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "CredibilityScore",
                table: "IncidentReports",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
