using DocAI.API.DTOs;

namespace DocAI.API.Services.Interfaces;

public interface IIcd10Service
{
    Task<List<CodedItem>> LookupCodesAsync(string diagnosisText);
}
