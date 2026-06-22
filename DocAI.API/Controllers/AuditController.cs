using System.Security.Claims;
using System.Text.Json;
using DocAI.API.Data;
using DocAI.API.DTOs;
using DocAI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly DocAIDbContext _db;
    private readonly IAuditEngineService _auditEngine;

    public AuditController(DocAIDbContext db, IAuditEngineService auditEngine)
    {
        _db = db;
        _auditEngine = auditEngine;
    }

    /// <summary>
    /// Generate (or regenerate) the AI audit report for a patient case.
    /// This calls OpenAI + ICD-10 + RxNorm + LOINC APIs in parallel.
    /// </summary>
    [HttpPost("{caseId:guid}/generate")]
    public async Task<IActionResult> GenerateAudit(Guid caseId)
    {
        var patientCase = await _db.PatientCases
            .Include(p => p.AuditReport)
            .FirstOrDefaultAsync(p => p.Id == caseId);

        if (patientCase == null) return NotFound(new { message = "Patient case not found." });

        // Remove existing audit if regenerating
        if (patientCase.AuditReport != null)
            _db.AuditReports.Remove(patientCase.AuditReport);

        Models.AuditReport report;
        try
        {
            report = await _auditEngine.GenerateAuditReportAsync(patientCase);
        }
        catch (Exception ex) when (ex.Message.Contains("insufficient_quota") || ex.Message.Contains("429"))
        {
            return StatusCode(503, new { message = "OpenAI quota exceeded. Please check your API key billing and try again." });
        }

        _db.AuditReports.Add(report);

        patientCase.Status = "PendingReview";
        patientCase.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(ToDto(report));
    }

    /// <summary>Get the audit report for a patient case.</summary>
    [HttpGet("{caseId:guid}")]
    public async Task<IActionResult> GetAudit(Guid caseId)
    {
        var report = await _db.AuditReports.FirstOrDefaultAsync(a => a.PatientCaseId == caseId);
        if (report == null) return NotFound(new { message = "No audit report found. Generate one first." });
        return Ok(ToDto(report));
    }

    private static AuditReportResponseDto ToDto(Models.AuditReport r) => new()
    {
        Id = r.Id,
        PatientCaseId = r.PatientCaseId,
        DocumentationReview = r.DocumentationReview,
        DocumentationScore = r.DocumentationScore,
        ClinicalConsistencyReview = r.ClinicalConsistencyReview,
        ClinicalConsistencyScore = r.ClinicalConsistencyScore,
        CarePlanReview = r.CarePlanReview,
        CarePlanScore = r.CarePlanScore,
        InsuranceRiskFlags = r.InsuranceRiskFlags,
        InsuranceRiskScore = r.InsuranceRiskScore,
        SuggestedImprovements = r.SuggestedImprovements,
        FinalSummary = r.FinalSummary,
        OverallAcceptanceRate = r.OverallAcceptanceRate,
        AcceptanceRationale = r.AcceptanceRationale,
        GeneratedAt = r.GeneratedAt,
        ModelUsed = r.ModelUsed,
        Icd10Codes  = Deserialize<CodedItem>(r.Icd10Codes),
        RxNormCodes = Deserialize<CodedItem>(r.RxNormCodes),
        LoincCodes  = Deserialize<CodedItem>(r.LoincCodes),
        SnomedCodes = Deserialize<CodedItem>(r.SnomedCodes),
        RecommendedLabs          = Deserialize<RecommendedItem>(r.RecommendedLabs),
        RecommendedImaging       = Deserialize<RecommendedItem>(r.RecommendedImaging),
        RecommendedProcedures    = Deserialize<RecommendedItem>(r.RecommendedProcedures),
        RecommendedConsultations = Deserialize<RecommendedItem>(r.RecommendedConsultations),
    };

    private static List<T> Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return JsonSerializer.Deserialize<List<T>>(json) ?? []; }
        catch { return []; }
    }
}
