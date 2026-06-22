using DocAI.API.DTOs;
using DocAI.API.Services.Interfaces;

namespace DocAI.API.Services;

/// <summary>Stub — replace with SNOMED CT Browser API or NLM SNOMED endpoint when credentials available.</summary>
public class SnomedStubService : ISnomedService
{
    public Task<List<CodedItem>> LookupConceptsAsync(string clinicalText)
    {
        // Stub: returns empty list. 
        // Real impl: POST to https://snowstorm.ihtsdotools.org/snowstorm/snomed-ct/MAIN/concepts
        return Task.FromResult(new List<CodedItem>());
    }
}

/// <summary>Stub — replace with AWS SDK Amazon.ComprehendMedical when AWS credentials available.</summary>
public class ComprehendMedicalStubService : IComprehendMedicalService
{
    public Task<string> ExtractMedicalEntitiesAsync(string clinicalText)
    {
        // Stub: returns empty JSON
        // Real impl: use AWSSDK.ComprehendMedical DetectEntitiesV2Async
        return Task.FromResult("{}");
    }
}

/// <summary>Stub — replace with UMLS REST API when UTS API Key available (https://uts.nlm.nih.gov).</summary>
public class UmlsStubService : IUmlsService
{
    public Task<List<CodedItem>> LookupTermsAsync(string clinicalText)
    {
        // Stub: returns empty list.
        // Real impl: GET https://uts-ws.nlm.nih.gov/rest/search/current?string={term}&apiKey={key}
        return Task.FromResult(new List<CodedItem>());
    }
}
