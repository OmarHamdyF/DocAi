using System.Text.Json;
using DocAI.API.Models;
using DocAI.API.Services.Interfaces;

namespace DocAI.API.Services;

public class AuditEngineService : IAuditEngineService
{
    private readonly IOpenAIService _openAI;
    private readonly IIcd10Service _icd10;
    private readonly IRxNormService _rxNorm;
    private readonly ILoincService _loinc;
    private readonly ISnomedService _snomed;
    private readonly IComprehendMedicalService _comprehend;
    private readonly IUmlsService _umls;
    private readonly ILogger<AuditEngineService> _logger;

    public AuditEngineService(
        IOpenAIService openAI, IIcd10Service icd10, IRxNormService rxNorm,
        ILoincService loinc, ISnomedService snomed, IComprehendMedicalService comprehend,
        IUmlsService umls, ILogger<AuditEngineService> logger)
    {
        _openAI = openAI; _icd10 = icd10; _rxNorm = rxNorm;
        _loinc = loinc; _snomed = snomed; _comprehend = comprehend;
        _umls = umls; _logger = logger;
    }

    public async Task<AuditReport> GenerateAuditReportAsync(PatientCase c)
    {
        _logger.LogInformation("Starting audit for case {CaseId}", c.Id);

        // Run all external API lookups in parallel alongside OpenAI
        var auditTask = _openAI.RunClinicalAuditAsync(BuildSystemPrompt(), BuildUserContent(c));
        var icd10Task = _icd10.LookupCodesAsync(c.ProvisionalDiagnosis);
        var rxNormTask = _rxNorm.LookupDrugCodesAsync(c.MedicationsPrescribed);
        var loincTask = _loinc.LookupLabCodesAsync(c.LabsRequested);
        var snomedTask = _snomed.LookupConceptsAsync(c.ProvisionalDiagnosis + " " + c.ChiefComplaint);
        var comprehendTask = _comprehend.ExtractMedicalEntitiesAsync(
            $"{c.ChiefComplaint} {c.Hopi} {c.ProvisionalDiagnosis}");
        var umlsTask = _umls.LookupTermsAsync(c.ProvisionalDiagnosis);

        await Task.WhenAll(auditTask, icd10Task, rxNormTask, loincTask, snomedTask, comprehendTask, umlsTask);

        var audit = await auditTask;
        var icd10 = await icd10Task;
        var rxNorm = await rxNormTask;
        var loinc = await loincTask;
        var snomed = await snomedTask;
        var comprehendResult = await comprehendTask;
        var umls = await umlsTask;

        return new AuditReport
        {
            PatientCaseId = c.Id,
            DocumentationReview = audit.DocumentationReview,
            DocumentationScore = audit.DocumentationScore,
            ClinicalConsistencyReview = audit.ClinicalConsistencyReview,
            ClinicalConsistencyScore = audit.ClinicalConsistencyScore,
            CarePlanReview = audit.CarePlanReview,
            CarePlanScore = audit.CarePlanScore,
            InsuranceRiskFlags = audit.InsuranceRiskFlags,
            InsuranceRiskScore = audit.InsuranceRiskScore,
            SuggestedImprovements = audit.SuggestedImprovements,
            FinalSummary = audit.FinalSummary,
            OverallAcceptanceRate = audit.OverallAcceptanceRate,
            AcceptanceRationale = audit.AcceptanceRationale,
            RecommendedLabs          = JsonSerializer.Serialize(audit.RecommendedLabs),
            RecommendedImaging       = JsonSerializer.Serialize(audit.RecommendedImaging),
            RecommendedProcedures    = JsonSerializer.Serialize(audit.RecommendedProcedures),
            RecommendedConsultations = JsonSerializer.Serialize(audit.RecommendedConsultations),
            Icd10Codes = JsonSerializer.Serialize(icd10),
            RxNormCodes = JsonSerializer.Serialize(rxNorm),
            LoincCodes = JsonSerializer.Serialize(loinc),
            SnomedCodes = JsonSerializer.Serialize(snomed),
            ComprehendEntities = comprehendResult,
            UmlsTerms = JsonSerializer.Serialize(umls),
            ModelUsed = "gpt-4o"
        };
    }

    private static string BuildSystemPrompt() => """
        You are a senior Clinical Documentation and Insurance Validation Copilot used inside a hospital HIS system.
        Your job is to assist physicians by reviewing clinical documentation before submission.

        IMPORTANT RULES:
        - You are NOT a treating physician.
        - You do NOT give final diagnoses.
        - You ONLY review, validate, and suggest improvements.
        - Be conservative; avoid hallucinations.
        - If data is missing, explicitly state it is missing.

        You must perform the following analysis and return a SINGLE valid JSON object with exactly these keys:
        {
          "documentation_review": "<Section 1: completeness of Chief Complaint, HOPI, Physical Exam, Progress Note>",
          "documentation_score": <0-100 integer>,
          "clinical_consistency_review": "<Section 2: Is diagnosis supported by symptoms, exam, investigations, medications?>",
          "clinical_consistency_score": <0-100 integer>,
          "care_plan_review": "<Section 3: Labs, imaging, procedures, medications — appropriate/missing/justified?>",
          "care_plan_score": <0-100 integer>,
          "insurance_risk_flags": "<Section 4: Missing/weak documentation that may cause claim rejection or medicolegal risk. Flag clearly with ⚠️>",
          "insurance_risk_score": <0-100 integer where 100=highest risk>,
          "suggested_improvements": "<Section 5: Rewrite/improve weak/missing documentation. Provide editable suggestions>",
          "final_summary": "<Section 6: Brief summary for physician with key action items>",
          "overall_acceptance_rate": <0-100 integer estimated insurance claim acceptance rate>,
          "acceptance_rationale": "<Section 7: Explain the acceptance rate score — what drives it up or down>",
          "recommended_labs": [
            { "name": "<lab test name>", "reason": "<clinical justification>", "urgency": "<Routine|Urgent|Stat>" }
          ],
          "recommended_imaging": [
            { "name": "<imaging study name>", "reason": "<clinical justification>", "urgency": "<Routine|Urgent|Stat>" }
          ],
          "recommended_procedures": [
            { "name": "<procedure name>", "reason": "<clinical justification>", "urgency": "<Routine|Urgent|Stat>" }
          ],
          "recommended_consultations": [
            { "name": "<specialty>", "reason": "<clinical justification>", "urgency": "<Routine|Urgent|Stat>" }
          ]
        }

        For the recommended_* arrays:
        - Include ONLY clinically indicated items that are currently missing from the case.
        - Each item must have a clear evidence-based reason tied to the patient's presentation.
        - Urgency must be exactly one of: Routine, Urgent, Stat.
        - Return an empty array [] if nothing is missing for that category.
        - These recommendations directly help the hospital order clinically justified, billable services.

        Be specific, clinical, and precise. Use medical terminology appropriately.
        """;

    private static string BuildUserContent(PatientCase c) => $"""
        PATIENT CASE
        Patient: {c.PatientName}, Age: {c.PatientAge}, Gender: {c.PatientGender}, ID: {c.PatientId}

        Chief Complaint:
        {OrMissing(c.ChiefComplaint)}

        History of Present Illness (HOPI):
        {OrMissing(c.Hopi)}

        Physical Examination:
        {OrMissing(c.PhysicalExam)}

        Progress Note:
        {OrMissing(c.ProgressNote)}

        Provisional Diagnosis:
        {OrMissing(c.ProvisionalDiagnosis)}

        Medications Prescribed:
        {OrMissing(c.MedicationsPrescribed)}

        Labs Requested:
        {OrMissing(c.LabsRequested)}

        Imaging Requested:
        {OrMissing(c.ImagingRequested)}

        Procedures Requested:
        {OrMissing(c.ProceduresRequested)}

        Laboratory Results:
        {OrMissing(c.LabResults)}

        Imaging Results:
        {OrMissing(c.ImagingResults)}

        Medication Dispensed:
        {OrMissing(c.MedicationDispensed)}

        Previous Visits (last 10 encounters):
        {OrMissing(c.PreviousVisits)}
        """;

    private static string OrMissing(string? val) =>
        string.IsNullOrWhiteSpace(val) ? "[NOT PROVIDED]" : val;
}
