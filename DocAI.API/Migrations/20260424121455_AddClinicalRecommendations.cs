using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAI.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicalRecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecommendedConsultations",
                table: "AuditReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedImaging",
                table: "AuditReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedLabs",
                table: "AuditReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedProcedures",
                table: "AuditReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecommendedConsultations",
                table: "AuditReports");

            migrationBuilder.DropColumn(
                name: "RecommendedImaging",
                table: "AuditReports");

            migrationBuilder.DropColumn(
                name: "RecommendedLabs",
                table: "AuditReports");

            migrationBuilder.DropColumn(
                name: "RecommendedProcedures",
                table: "AuditReports");
        }
    }
}
