namespace DocAI.API.DTOs;

public class ApprovalDto
{
    public Guid PatientCaseId { get; set; }
    public string Action { get; set; } = string.Empty; // Approved | Rejected | EditedAndApproved
    public string Comments { get; set; } = string.Empty;
    public string ApprovedImprovements { get; set; } = string.Empty;
}

public class ApprovalResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientCaseId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
    public string ApprovedImprovements { get; set; } = string.Empty;
    public string PhysicianName { get; set; } = string.Empty;
    public DateTime ActionAt { get; set; }
    public string CaseStatus { get; set; } = string.Empty;
}
