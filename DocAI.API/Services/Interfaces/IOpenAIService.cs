using DocAI.API.DTOs;

namespace DocAI.API.Services.Interfaces;

public interface IOpenAIService
{
    Task<ClinicalAuditResult> RunClinicalAuditAsync(string systemPrompt, string userContent);
}

public class ClinicalAuditResult
{
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
    public List<AuditRecommendedItem> RecommendedLabs { get; set; } = new();
    public List<AuditRecommendedItem> RecommendedImaging { get; set; } = new();
    public List<AuditRecommendedItem> RecommendedProcedures { get; set; } = new();
    public List<AuditRecommendedItem> RecommendedConsultations { get; set; } = new();
}

public class AuditRecommendedItem
{
    public string Name { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Urgency { get; set; } = "Routine";
}
