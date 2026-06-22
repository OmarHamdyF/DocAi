namespace DocAI.API.Models;

public class ApprovalRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientCaseId { get; set; }
    public PatientCase? PatientCase { get; set; }

    public Guid PhysicianId { get; set; }
    public User? Physician { get; set; }

    public string Action { get; set; } = string.Empty; // Approved | Rejected | EditedAndApproved
    public string Comments { get; set; } = string.Empty;

    // Physician-edited improvements (free text stored as JSON)
    public string ApprovedImprovements { get; set; } = string.Empty;

    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
}
