using DocAI.API.DTOs;

namespace DocAI.API.Services.Interfaces;

public interface ISnomedService
{
    Task<List<CodedItem>> LookupConceptsAsync(string clinicalText);
}

public interface IComprehendMedicalService
{
    Task<string> ExtractMedicalEntitiesAsync(string clinicalText);
}

public interface IUmlsService
{
    Task<List<CodedItem>> LookupTermsAsync(string clinicalText);
}
