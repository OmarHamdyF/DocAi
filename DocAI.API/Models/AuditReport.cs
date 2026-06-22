namespace DocAI.API.Models;

public class AuditReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientCaseId { get; set; }
    public PatientCase? PatientCase { get; set; }

    // Section 1: Documentation Review
    public string DocumentationReview { get; set; } = string.Empty;
    public int DocumentationScore { get; set; } // 0-100

    // Section 2: Clinical Consistency
    public string ClinicalConsistencyReview { get; set; } = string.Empty;
    public int ClinicalConsistencyScore { get; set; }

    // Section 3: Care Plan Review
    public string CarePlanReview { get; set; } = string.Empty;
    public int CarePlanScore { get; set; }

    // Section 4: Insurance Risk Flags
    public string InsuranceRiskFlags { get; set; } = string.Empty;
    public int InsuranceRiskScore { get; set; } // 0-100 (higher = higher risk)

    // Section 5: Suggested Improvements
    public string SuggestedImprovements { get; set; } = string.Empty;

    // Section 6: Final Summary
    public string FinalSummary { get; set; } = string.Empty;

    // Section 7: Acceptance Rate
    public int OverallAcceptanceRate { get; set; } // 0-100 estimated claim acceptance %
    public string AcceptanceRationale { get; set; } = string.Empty;

    // Section 8: Clinical Recommendations
    public string RecommendedLabs { get; set; } = string.Empty;            // JSON array of RecommendedItem
    public string RecommendedImaging { get; set; } = string.Empty;         // JSON array of RecommendedItem
    public string RecommendedProcedures { get; set; } = string.Empty;      // JSON array of RecommendedItem
    public string RecommendedConsultations { get; set; } = string.Empty;   // JSON array of RecommendedItem

    // Coded data from external APIs
    public string Icd10Codes { get; set; } = string.Empty;       // JSON array
    public string RxNormCodes { get; set; } = string.Empty;      // JSON array
    public string LoincCodes { get; set; } = string.Empty;       // JSON array
    public string SnomedCodes { get; set; } = string.Empty;      // JSON array (stub)
    public string ComprehendEntities { get; set; } = string.Empty; // JSON (stub)
    public string UmlsTerms { get; set; } = string.Empty;        // JSON array (stub)

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public string ModelUsed { get; set; } = "gpt-4o";
}
