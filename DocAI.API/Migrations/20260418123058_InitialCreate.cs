using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocAI.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientAge = table.Column<int>(type: "int", nullable: false),
                    PatientGender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChiefComplaint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Hopi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhysicalExam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgressNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProvisionalDiagnosis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MedicationsPrescribed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabsRequested = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagingRequested = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProceduresRequested = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagingResults = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicationDispensed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousVisits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCases_Users_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApprovedImprovements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRecords_PatientCases_PatientCaseId",
                        column: x => x.PatientCaseId,
                        principalTable: "PatientCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRecords_Users_PhysicianId",
                        column: x => x.PhysicianId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentationReview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentationScore = table.Column<int>(type: "int", nullable: false),
                    ClinicalConsistencyReview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClinicalConsistencyScore = table.Column<int>(type: "int", nullable: false),
                    CarePlanReview = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarePlanScore = table.Column<int>(type: "int", nullable: false),
                    InsuranceRiskFlags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsuranceRiskScore = table.Column<int>(type: "int", nullable: false),
                    SuggestedImprovements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverallAcceptanceRate = table.Column<int>(type: "int", nullable: false),
                    AcceptanceRationale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icd10Codes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RxNormCodes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoincCodes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SnomedCodes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComprehendEntities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UmlsTerms = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModelUsed = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditReports_PatientCases_PatientCaseId",
                        column: x => x.PatientCaseId,
                        principalTable: "PatientCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecords_PatientCaseId",
                table: "ApprovalRecords",
                column: "PatientCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRecords_PhysicianId",
                table: "ApprovalRecords",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditReports_PatientCaseId",
                table: "AuditReports",
                column: "PatientCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_PhysicianId",
                table: "PatientCases",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRecords");

            migrationBuilder.DropTable(
                name: "AuditReports");

            migrationBuilder.DropTable(
                name: "PatientCases");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
