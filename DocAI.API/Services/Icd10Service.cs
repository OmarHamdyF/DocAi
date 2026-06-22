using System.Text.Json;
using DocAI.API.DTOs;
using DocAI.API.Services.Interfaces;

namespace DocAI.API.Services;

/// <summary>
/// Uses NLM Clinical Table Search — free, no API key required.
/// Docs: https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search
/// </summary>
public class Icd10Service : IIcd10Service
{
    private readonly HttpClient _http;
    private readonly ILogger<Icd10Service> _logger;

    public Icd10Service(IHttpClientFactory factory, ILogger<Icd10Service> logger)
    {
        _http = factory.CreateClient("Icd10");
        _logger = logger;
    }

    public async Task<List<CodedItem>> LookupCodesAsync(string diagnosisText)
    {
        if (string.IsNullOrWhiteSpace(diagnosisText)) return new();

        // Take first meaningful words to avoid overly long queries
        var query = diagnosisText.Length > 100 ? diagnosisText[..100] : diagnosisText;
        var url = $"https://clinicaltables.nlm.nih.gov/api/icd10cm/v3/search?sf=code,name&terms={Uri.EscapeDataString(query)}&maxList=5";

        try
        {
            var response = await _http.GetStringAsync(url);
            return ParseNlmResponse(response, "ICD-10-CM");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ICD-10 lookup failed for: {Text}", diagnosisText[..Math.Min(50, diagnosisText.Length)]);
            return new();
        }
    }

    private static List<CodedItem> ParseNlmResponse(string json, string system)
    {
        // NLM format: [total, [codes], null, [[code, name], ...]]
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() < 4) return new();

        var items = new List<CodedItem>();
        var dataArray = root[3];
        if (dataArray.ValueKind != JsonValueKind.Array) return items;

        foreach (var item in dataArray.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Array && item.GetArrayLength() >= 2)
            {
                items.Add(new CodedItem
                {
                    Code = item[0].GetString() ?? string.Empty,
                    Display = item[1].GetString() ?? string.Empty,
                    System = system
                });
            }
        }
        return items;
    }
}
