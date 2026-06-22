using DocAI.API.DTOs;
using DocAI.API.Models;

namespace DocAI.API.Services.Interfaces;

public interface IAuditEngineService
{
    Task<AuditReport> GenerateAuditReportAsync(PatientCase patientCase);
}
