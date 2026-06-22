using DocAI.API.DTOs;

namespace DocAI.API.Services.Interfaces;

public interface ILoincService
{
    Task<List<CodedItem>> LookupLabCodesAsync(string labText);
}
