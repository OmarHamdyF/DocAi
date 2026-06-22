using System.Text.Json;
using DocAI.API.DTOs;
using DocAI.API.Services.Interfaces;

namespace DocAI.API.Services;

/// <summary>
/// Uses LOINC Clinical Table Search — free, no API key required.
/// Docs: https://clinicaltables.nlm.nih.gov/api/loinc_items/v3/search
/// </summary>
public class LoincService : ILoincService
{
    private readonly HttpClient _http;
    private readonly ILogger<LoincService> _logger;

    public LoincService(IHttpClientFactory factory, ILogger<LoincService> logger)
    {
        _http = factory.CreateClient("Loinc");
        _logger = logger;
    }

    public async Task<List<CodedItem>> LookupLabCodesAsync(string labText)
    {
        if (string.IsNullOrWhiteSpace(labText)) return new();

        var query = labText.Length > 100 ? labText[..100] : labText;
        var url = $"https://clinicaltables.nlm.nih.gov/api/loinc_items/v3/search?sf=LOINC_NUM,LONG_COMMON_NAME&terms={Uri.EscapeDataString(query)}&maxList=5&type=question";

        try
        {
            var response = await _http.GetStringAsync(url);
            return ParseLoincResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "LOINC lookup failed for: {Text}", labText[..Math.Min(50, labText.Length)]);
            return new();
        }
    }

    private static List<CodedItem> ParseLoincResponse(string json)
    {
        // NLM format: [total, [codes], null, [[loinc_num, name], ...]]
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
                    System = "LOINC"
                });
            }
        }
        return items;
    }
}
