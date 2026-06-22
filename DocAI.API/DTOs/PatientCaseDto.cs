namespace DocAI.API.DTOs;

public class PatientCaseDto
{
    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string PatientGender { get; set; } = string.Empty;
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
}

public class PatientCaseResponseDto
{
    public Guid Id { get; set; }
    public string PatientId { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string PatientGender { get; set; } = string.Empty;
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
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string PhysicianName { get; set; } = string.Empty;
    public AuditReportResponseDto? AuditReport { get; set; }
}
