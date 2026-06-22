using System.Text;
using System.Text.Json;
using DocAI.API.Services.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace DocAI.API.Services;

public class OpenAIService : IOpenAIService
{
    private readonly ChatClient _client;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration config, ILogger<OpenAIService> logger)
    {
        _logger = logger;
        var apiKey = config["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey not configured");
        var model = config["OpenAI:Model"] ?? "gpt-4o";
        var openAiClient = new OpenAIClient(apiKey);
        _client = openAiClient.GetChatClient(model);
    }

    public async Task<ClinicalAuditResult> RunClinicalAuditAsync(string systemPrompt, string userContent)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userContent)
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            };

            var response = await _client.CompleteChatAsync(messages, options);
            var json = response.Value.Content[0].Text;

            _logger.LogInformation("OpenAI audit response received ({Length} chars)", json.Length);

            return ParseAuditResult(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI audit call failed");
            throw;
        }
    }

    private static ClinicalAuditResult ParseAuditResult(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        return new ClinicalAuditResult
        {
            DocumentationReview = GetString(root, "documentation_review"),
            DocumentationScore = GetInt(root, "documentation_score"),
            ClinicalConsistencyReview = GetString(root, "clinical_consistency_review"),
            ClinicalConsistencyScore = GetInt(root, "clinical_consistency_score"),
            CarePlanReview = GetString(root, "care_plan_review"),
            CarePlanScore = GetInt(root, "care_plan_score"),
            InsuranceRiskFlags = GetString(root, "insurance_risk_flags"),
            InsuranceRiskScore = GetInt(root, "insurance_risk_score"),
            SuggestedImprovements = GetString(root, "suggested_improvements"),
            FinalSummary = GetString(root, "final_summary"),
            OverallAcceptanceRate = GetInt(root, "overall_acceptance_rate"),
            AcceptanceRationale = GetString(root, "acceptance_rationale"),
            RecommendedLabs = GetRecommendedItems(root, "recommended_labs"),
            RecommendedImaging = GetRecommendedItems(root, "recommended_imaging"),
            RecommendedProcedures = GetRecommendedItems(root, "recommended_procedures"),
            RecommendedConsultations = GetRecommendedItems(root, "recommended_consultations"),
        };
    }

    private static List<AuditRecommendedItem> GetRecommendedItems(JsonElement root, string key)
    {
        if (!root.TryGetProperty(key, out var arr) || arr.ValueKind != JsonValueKind.Array)
            return [];

        return arr.EnumerateArray().Select(el => new AuditRecommendedItem
        {
            Name    = el.TryGetProperty("name",    out var n) ? n.GetString() ?? "" : "",
            Reason  = el.TryGetProperty("reason",  out var r) ? r.GetString() ?? "" : "",
            Urgency = el.TryGetProperty("urgency", out var u) ? u.GetString() ?? "Routine" : "Routine",
        }).ToList();
    }

    private static string GetString(JsonElement root, string key) =>
        root.TryGetProperty(key, out var el) ? el.GetString() ?? string.Empty : string.Empty;

    private static int GetInt(JsonElement root, string key) =>
        root.TryGetProperty(key, out var el) && el.TryGetInt32(out var val) ? val : 0;
}
