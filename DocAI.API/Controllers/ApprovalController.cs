using System.Security.Claims;
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
public class ApprovalController : ControllerBase
{
    private readonly DocAIDbContext _db;

    public ApprovalController(DocAIDbContext db) => _db = db;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Physician approves, rejects, or approves with edits.
    /// Actions: Approved | Rejected | EditedAndApproved
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitApproval([FromBody] ApprovalDto dto)
    {
        var patientCase = await _db.PatientCases.FindAsync(dto.PatientCaseId);
        if (patientCase == null) return NotFound(new { message = "Patient case not found." });

        if (patientCase.Status == "Approved")
            return BadRequest(new { message = "Case is already approved." });

        var validActions = new[] { "Approved", "Rejected", "EditedAndApproved" };
        if (!validActions.Contains(dto.Action))
            return BadRequest(new { message = $"Invalid action. Must be one of: {string.Join(", ", validActions)}" });

        var record = new ApprovalRecord
        {
            PatientCaseId = dto.PatientCaseId,
            PhysicianId = CurrentUserId,
            Action = dto.Action,
            Comments = dto.Comments,
            ApprovedImprovements = dto.ApprovedImprovements
        };

        _db.ApprovalRecords.Add(record);

        patientCase.Status = dto.Action switch
        {
            "Approved" => "Approved",
            "EditedAndApproved" => "Approved",
            "Rejected" => "Rejected",
            _ => patientCase.Status
        };
        patientCase.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new ApprovalResponseDto
        {
            Id = record.Id,
            PatientCaseId = record.PatientCaseId,
            Action = record.Action,
            Comments = record.Comments,
            ApprovedImprovements = record.ApprovedImprovements,
            ActionAt = record.ActionAt,
            CaseStatus = patientCase.Status
        });
    }

    /// <summary>Get approval history for a patient case.</summary>
    [HttpGet("{caseId:guid}")]
    public async Task<IActionResult> GetHistory(Guid caseId)
    {
        var records = await _db.ApprovalRecords
            .Include(a => a.Physician)
            .Where(a => a.PatientCaseId == caseId)
            .OrderByDescending(a => a.ActionAt)
            .Select(a => new ApprovalResponseDto
            {
                Id = a.Id,
                PatientCaseId = a.PatientCaseId,
                Action = a.Action,
                Comments = a.Comments,
                ApprovedImprovements = a.ApprovedImprovements,
                PhysicianName = a.Physician != null ? a.Physician.FullName : "",
                ActionAt = a.ActionAt,
                CaseStatus = ""
            })
            .ToListAsync();

        return Ok(records);
    }

    /// <summary>Dashboard stats: counts by status.</summary>
    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _db.PatientCases
            .GroupBy(p => p.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var avgAcceptance = await _db.AuditReports
            .AverageAsync(a => (double?)a.OverallAcceptanceRate) ?? 0;

        return Ok(new { stats, averageAcceptanceRate = Math.Round(avgAcceptance, 1) });
    }
}
