using DocAI.API.DTOs;

namespace DocAI.API.Services.Interfaces;

public interface IRxNormService
{
    Task<List<CodedItem>> LookupDrugCodesAsync(string medicationText);
}
