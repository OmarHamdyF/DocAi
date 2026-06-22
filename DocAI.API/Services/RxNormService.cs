using System.Text.Json;
using DocAI.API.DTOs;
using DocAI.API.Services.Interfaces;

namespace DocAI.API.Services;

/// <summary>
/// Uses NLM RxNorm API — free, no API key required.
/// Docs: https://rxnav.nlm.nih.gov/RxNormAPIs.html
/// </summary>
public class RxNormService : IRxNormService
{
    private readonly HttpClient _http;
    private readonly ILogger<RxNormService> _logger;

    public RxNormService(IHttpClientFactory factory, ILogger<RxNormService> logger)
    {
        _http = factory.CreateClient("RxNorm");
        _logger = logger;
    }

    public async Task<List<CodedItem>> LookupDrugCodesAsync(string medicationText)
    {
        if (string.IsNullOrWhiteSpace(medicationText)) return new();

        var results = new List<CodedItem>();

        // Split by common delimiters to handle multiple medications
        var medications = medicationText
            .Split(new[] { ',', '\n', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(m => m.Trim())
            .Where(m => m.Length > 2)
            .Take(5);

        foreach (var med in medications)
        {
            try
            {
                var url = $"https://rxnav.nlm.nih.gov/REST/drugs.json?name={Uri.EscapeDataString(med)}";
                var response = await _http.GetStringAsync(url);
                var items = ParseRxNormResponse(response);
                results.AddRange(items);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RxNorm lookup failed for: {Med}", med);
            }
        }

        return results.DistinctBy(r => r.Code).ToList();
    }

    private static List<CodedItem> ParseRxNormResponse(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var items = new List<CodedItem>();

        if (!root.TryGetProperty("drugGroup", out var group)) return items;
        if (!group.TryGetProperty("conceptGroup", out var groups)) return items;

        foreach (var grp in groups.EnumerateArray())
        {
            if (!grp.TryGetProperty("conceptProperties", out var props)) continue;
            foreach (var prop in props.EnumerateArray())
            {
                var rxcui = prop.TryGetProperty("rxcui", out var c) ? c.GetString() ?? "" : "";
                var name = prop.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                if (!string.IsNullOrEmpty(rxcui))
                {
                    items.Add(new CodedItem { Code = rxcui, Display = name, System = "RxNorm" });
                }
            }
        }
        return items.Take(5).ToList();
    }
}
