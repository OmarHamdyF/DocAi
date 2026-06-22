using System.Security.Claims;
using System.Text.Json;
using DocAI.API.Data;
using DocAI.API.DTOs;
using DocAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientCasesController : ControllerBase
{
    private readonly DocAIDbContext _db;

    public PatientCasesController(DocAIDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Create a new patient case (saves as Draft).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientCaseDto dto)
    {
        var patientCase = new PatientCase
        {
            PatientId = dto.PatientId,
            PatientName = dto.PatientName,
            PatientAge = dto.PatientAge,
            PatientGender = dto.PatientGender,
            ChiefComplaint = dto.ChiefComplaint,
            Hopi = dto.Hopi,
            PhysicalExam = dto.PhysicalExam,
            ProgressNote = dto.ProgressNote,
            ProvisionalDiagnosis = dto.ProvisionalDiagnosis,
            MedicationsPrescribed = dto.MedicationsPrescribed,
            LabsRequested = dto.LabsRequested,
            ImagingRequested = dto.ImagingRequested,
            ProceduresRequested = dto.ProceduresRequested,
            LabResults = dto.LabResults,
            ImagingResults = dto.ImagingResults,
            MedicationDispensed = dto.MedicationDispensed,
            PreviousVisits = dto.PreviousVisits,
            PhysicianId = CurrentUserId,
            Status = "Draft"
        };

        _db.PatientCases.Add(patientCase);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = patientCase.Id }, MapToResponse(patientCase));
    }

    /// <summary>Get a patient case by ID (with audit report if generated).</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var c = await _db.PatientCases
            .Include(p => p.Physician)
            .Include(p => p.AuditReport)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (c == null) return NotFound();
        return Ok(MapToResponse(c));
    }

    /// <summary>List all cases for the current physician (paginated).</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = _db.PatientCases
            .Include(p => p.Physician)
            .Include(p => p.AuditReport)
            .OrderByDescending(p => p.CreatedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { total, page, pageSize, items = items.Select(MapToResponse) });
    }

    /// <summary>Update a patient case (only while in Draft status).</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PatientCaseDto dto)
    {
        var c = await _db.PatientCases.FindAsync(id);
        if (c == null) return NotFound();
        if (c.Status != "Draft") return BadRequest(new { message = "Only Draft cases can be updated." });

        c.PatientId = dto.PatientId;
        c.PatientName = dto.PatientName;
        c.PatientAge = dto.PatientAge;
        c.PatientGender = dto.PatientGender;
        c.ChiefComplaint = dto.ChiefComplaint;
        c.Hopi = dto.Hopi;
        c.PhysicalExam = dto.PhysicalExam;
        c.ProgressNote = dto.ProgressNote;
        c.ProvisionalDiagnosis = dto.ProvisionalDiagnosis;
        c.MedicationsPrescribed = dto.MedicationsPrescribed;
        c.LabsRequested = dto.LabsRequested;
        c.ImagingRequested = dto.ImagingRequested;
        c.ProceduresRequested = dto.ProceduresRequested;
        c.LabResults = dto.LabResults;
        c.ImagingResults = dto.ImagingResults;
        c.MedicationDispensed = dto.MedicationDispensed;
        c.PreviousVisits = dto.PreviousVisits;
        c.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(MapToResponse(c));
    }

    private static PatientCaseResponseDto MapToResponse(PatientCase c)
    {
        var dto = new PatientCaseResponseDto
        {
            Id = c.Id,
            PatientId = c.PatientId,
            PatientName = c.PatientName,
            PatientAge = c.PatientAge,
            PatientGender = c.PatientGender,
            ChiefComplaint = c.ChiefComplaint,
            Hopi = c.Hopi,
            PhysicalExam = c.PhysicalExam,
            ProgressNote = c.ProgressNote,
            ProvisionalDiagnosis = c.ProvisionalDiagnosis,
            MedicationsPrescribed = c.MedicationsPrescribed,
            LabsRequested = c.LabsRequested,
            ImagingRequested = c.ImagingRequested,
            ProceduresRequested = c.ProceduresRequested,
            LabResults = c.LabResults,
            ImagingResults = c.ImagingResults,
            MedicationDispensed = c.MedicationDispensed,
            PreviousVisits = c.PreviousVisits,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            PhysicianName = c.Physician?.FullName ?? string.Empty
        };

        if (c.AuditReport != null)
            dto.AuditReport = MapAuditReport(c.AuditReport);

        return dto;
    }

    private static AuditReportResponseDto MapAuditReport(AuditReport a)
    {
        static List<CodedItem> Decode(string json)
        {
            if (string.IsNullOrEmpty(json)) return new();
            try { return JsonSerializer.Deserialize<List<CodedItem>>(json) ?? new(); }
            catch { return new(); }
        }

        return new AuditReportResponseDto
        {
            Id = a.Id,
            PatientCaseId = a.PatientCaseId,
            DocumentationReview = a.DocumentationReview,
            DocumentationScore = a.DocumentationScore,
            ClinicalConsistencyReview = a.ClinicalConsistencyReview,
            ClinicalConsistencyScore = a.ClinicalConsistencyScore,
            CarePlanReview = a.CarePlanReview,
            CarePlanScore = a.CarePlanScore,
            InsuranceRiskFlags = a.InsuranceRiskFlags,
            InsuranceRiskScore = a.InsuranceRiskScore,
            SuggestedImprovements = a.SuggestedImprovements,
            FinalSummary = a.FinalSummary,
            OverallAcceptanceRate = a.OverallAcceptanceRate,
            AcceptanceRationale = a.AcceptanceRationale,
            Icd10Codes = Decode(a.Icd10Codes),
            RxNormCodes = Decode(a.RxNormCodes),
            LoincCodes = Decode(a.LoincCodes),
            SnomedCodes = Decode(a.SnomedCodes),
            GeneratedAt = a.GeneratedAt,
            ModelUsed = a.ModelUsed
        };
    }
}
