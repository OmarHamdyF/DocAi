namespace DocAI.API.Models;

public class PatientCase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string PatientGender { get; set; } = string.Empty;

    // Clinical Documentation Fields
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Hopi { get; set; } = string.Empty;
    public string PhysicalExam { get; set; } = string.Empty;
    public string ProgressNote { get; set; } = string.Empty;
    public string ProvisionalDiagnosis { get; set; } = string.Empty;
    public string MedicationsPrescribed { get; set; } = string.Empty;
    public string LabsRequested { get; set; } = string.Empty;
    public string ImagingRequested { get; set; } = string.Empty;
    public string ProceduresRequested { get; set; } = string.Empty;
    public string LabResults { get; set; } = string.Empty;
    public string ImagingResults { get; set; } = string.Empty;
    public string MedicationDispensed { get; set; } = string.Empty;
    public string PreviousVisits { get; set; } = string.Empty;

    // Metadata
    public string Status { get; set; } = "Draft"; // Draft | PendingReview | Approved | Rejected
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid PhysicianId { get; set; }
    public User? Physician { get; set; }

    public AuditReport? AuditReport { get; set; }
    public ICollection<ApprovalRecord> ApprovalRecords { get; set; } = new List<ApprovalRecord>();
}
