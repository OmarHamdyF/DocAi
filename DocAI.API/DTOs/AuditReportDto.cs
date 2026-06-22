namespace DocAI.API.DTOs;

public class AuditReportResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientCaseId { get; set; }

    public string DocumentationReview { get; set; } = string.Empty;
    public int DocumentationScore { get; set; }

    public string ClinicalConsistencyReview { get; set; } = string.Empty;
    public int ClinicalConsistencyScore { get; set; }

    public string CarePlanReview { get; set; } = string.Empty;
    public int CarePlanScore { get; set; }

    public string InsuranceRiskFlags { get; set; } = string.Empty;
    public int InsuranceRiskScore { get; set; }

    public string SuggestedImprovements { get; set; } = string.Empty;
    public string FinalSummary { get; set; } = string.Empty;

    public int OverallAcceptanceRate { get; set; }
    public string AcceptanceRationale { get; set; } = string.Empty;

    public List<RecommendedItem> RecommendedLabs { get; set; } = new();
    public List<RecommendedItem> RecommendedImaging { get; set; } = new();
    public List<RecommendedItem> RecommendedProcedures { get; set; } = new();
    public List<RecommendedItem> RecommendedConsultations { get; set; } = new();

    public List<CodedItem> Icd10Codes { get; set; } = new();
    public List<CodedItem> RxNormCodes { get; set; } = new();
    public List<CodedItem> LoincCodes { get; set; } = new();
    public List<CodedItem> SnomedCodes { get; set; } = new();

    public DateTime GeneratedAt { get; set; }
    public string ModelUsed { get; set; } = string.Empty;
}

public class RecommendedItem
{
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Urgency { get; set; } = "Routine"; // Routine | Urgent | Stat
}

public class CodedItem
{
    public string Code { get; set; } = string.Empty;
    public string Display { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
}
