# DocAI — Clinical Documentation & Insurance Validation Copilot

> AI-powered full-stack hospital HIS assistant for physicians. Validates clinical documentation, detects insurance risk, and maximizes claim acceptance rates.

---

## 🏗️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 10 ASP.NET Core Web API |
| Frontend | Angular 21 + PrimeNG |
| Database | SQL Server (EF Core 10) |
| Auth | JWT Bearer |
| AI Engine | OpenAI GPT-4o |
| Medical Coding | ICD-10 · RxNorm · LOINC (free NLM APIs) |
| Stubs (pluggable) | SNOMED CT · Amazon Comprehend Medical · UMLS |

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 20+
- SQL Server (local or Docker)
- OpenAI API key

### 1. Configure the Backend

Edit `DocAI.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DocAI;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_32_CHARACTER_SECRET_KEY_HERE",
    "Issuer": "DocAI",
    "Audience": "DocAI",
    "ExpiresHours": "8"
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key",
    "Model": "gpt-4o"
  }
}
```

> ⚠️ Never commit real API keys. Use `dotnet user-secrets` or environment variables in production.

### 2. Run the Backend

```bash
cd DocAI.API
dotnet run
# API: https://localhost:5001
# Scalar UI: https://localhost:5001/scalar/v1
```

The database is auto-migrated on first run.

### 3. Run the Frontend

```bash
cd DocAI.Angular
npm install
ng serve
# App: http://localhost:4200
```

---

## 📋 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/auth/login | Authenticate → JWT |
| POST | /api/auth/register | Register physician/auditor |
| POST | /api/patientcases | Create patient case |
| GET | /api/patientcases | List cases (paginated) |
| GET | /api/patientcases/{id} | Get case with audit report |
| PUT | /api/patientcases/{id} | Update draft case |
| POST | /api/audit/{caseId}/generate | 🤖 Generate AI audit report |
| GET | /api/audit/{caseId} | Get audit report |
| POST | /api/approval | Submit physician approval |
| GET | /api/approval/{caseId} | Get approval history |
| GET | /api/approval/dashboard/stats | Dashboard statistics |

---

## 🤖 AI Audit Report Sections

1. **Documentation Review** — Completeness check (Chief Complaint, HOPI, Physical Exam, Progress Note)
2. **Clinical Consistency Review** — Diagnosis vs symptoms/labs/medications alignment
3. **Care Plan Review** — Labs, imaging, procedures, medications appropriateness
4. **Insurance Risk Flags** — ⚠️ Missing/weak docs that may cause claim rejection
5. **Suggested Improvements** — Editable text for physician approval
6. **Final Summary** — Key action items for physician
7. **Acceptance Rate** — 0-100% estimated insurance claim acceptance + rationale

---

## 🔌 API Integrations

| API | Status | Notes |
|-----|--------|-------|
| OpenAI GPT-4o | ✅ Active | Clinical audit engine |
| NLM ICD-10 | ✅ Active | Free — no API key needed |
| NLM RxNorm | ✅ Active | Free — no API key needed |
| NLM LOINC | ✅ Active | Free — no API key needed |
| SNOMED CT | 🔌 Stub | Implement `ISnomedService` |
| Amazon Comprehend Medical | 🔌 Stub | Implement `IComprehendMedicalService` |
| UMLS | 🔌 Stub | Implement `IUmlsService` |

---

## 📁 Project Structure

```
DocAi/
├── DocAI.API/                    ← .NET 10 Backend
│   ├── Controllers/              
│   │   ├── AuthController.cs     
│   │   ├── PatientCasesController.cs
│   │   ├── AuditController.cs    
│   │   └── ApprovalController.cs 
│   ├── Services/
│   │   ├── Interfaces/           ← Service contracts
│   │   ├── OpenAIService.cs      ← GPT-4o integration
│   │   ├── Icd10Service.cs       ← NLM ICD-10
│   │   ├── RxNormService.cs      ← NLM RxNorm
│   │   ├── LoincService.cs       ← NLM LOINC
│   │   ├── ExternalStubServices.cs ← SNOMED/Comprehend/UMLS stubs
│   │   └── AuditEngineService.cs ← Orchestrator (all APIs parallel)
│   ├── Models/                   ← EF Core entities
│   ├── DTOs/                     ← Request/Response objects
│   ├── Data/                     ← DbContext + Migrations
│   └── Middleware/               ← Exception handling
└── DocAI.Angular/                ← Angular 21 Frontend
    └── src/app/
        ├── core/                 ← Guards, interceptors, services
        ├── features/             ← Pages/feature modules
        │   ├── auth/             ← Login page
        │   ├── dashboard/        ← Stats + recent cases
        │   ├── case-entry/       ← Multi-tab patient case form
        │   ├── audit/            ← AI audit report viewer
        │   ├── approval/         ← Approval workflow
        │   └── cases-list/       ← Paginated cases table
        └── shared/               ← Models, layout, reusable components
```

---

## 🔒 Roles

| Role | Access |
|------|--------|
| Physician | Create/edit cases, generate audits, approve/reject |
| Auditor | View all cases, view reports |
| Admin | Full access |
