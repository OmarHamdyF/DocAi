# DocAI — Complete User Guide
### Clinical Documentation & Insurance Validation Copilot

---

## Table of Contents

1. [What is DocAI?](#1-what-is-docai)
2. [System Requirements & Setup](#2-system-requirements--setup)
3. [Starting the Application](#3-starting-the-application)
4. [First-Time Setup: Register an Account](#4-first-time-setup-register-an-account)
5. [Login](#5-login)
6. [The Dashboard](#6-the-dashboard)
7. [Creating a Patient Case (Step-by-Step)](#7-creating-a-patient-case-step-by-step)
8. [Generating the AI Audit Report](#8-generating-the-ai-audit-report)
9. [Understanding the Audit Report](#9-understanding-the-audit-report)
10. [Approval Workflow](#10-approval-workflow)
11. [Cases List (All Cases)](#11-cases-list-all-cases)
12. [Complete End-to-End Workflow](#12-complete-end-to-end-workflow)
13. [API Reference (for developers)](#13-api-reference-for-developers)
14. [Troubleshooting](#14-troubleshooting)
15. [Roles & Permissions](#15-roles--permissions)

---

## 1. What is DocAI?

DocAI is an AI-powered clinical documentation assistant built for use inside hospital HIS (Hospital Information Systems). It helps physicians:

- **Document** patient cases completely and consistently
- **Validate** clinical documentation before submission
- **Detect** insurance risk flags that could cause claim rejection
- **Get AI suggestions** to improve documentation quality
- **Approve/Reject** AI suggestions before final submission
- **Maximize** insurance claim acceptance rates

> ⚠️ DocAI is NOT a treating physician and does NOT give final diagnoses. It only reviews, validates, and suggests improvements.

### The 7-Step AI Audit Process
Every patient case goes through 7 validation steps:

| Step | Name | What it checks |
|------|------|---------------|
| 1 | Documentation Review | Chief Complaint, HOPI, Physical Exam, Progress Note completeness |
| 2 | Clinical Consistency | Diagnosis vs symptoms, labs, medications alignment |
| 3 | Care Plan Review | Labs, imaging, procedures, medications appropriateness |
| 4 | Insurance Risk Flags | Missing/weak docs that could cause claim rejection |
| 5 | Suggested Improvements | Editable rewrites of weak documentation |
| 6 | Final Summary | Key action items for the physician |
| 7 | Acceptance Rate | 0–100% estimated insurance claim acceptance score |

---

## 2. System Requirements & Setup

### What you need installed
- **SQL Server Express** (already installed on your machine)
- **.NET 10 SDK**
- **Node.js 20+** and **npm**
- **OpenAI API Key** (from platform.openai.com)

### Configuration (one-time)
Open the file: `DocAI.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=DocAI;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "3AdgB-m#y$kNrW%G2v=R1UxZ!eH4IMYC09LlJ+@cOziPwSX*"
  },
  "OpenAI": {
    "ApiKey": "sk-proj-YOUR_KEY_HERE",
    "Model": "gpt-4o"
  }
}
```

Replace `sk-proj-YOUR_KEY_HERE` with your OpenAI API key.

---

## 3. Starting the Application

You need to start **two servers**: the backend (.NET API) and the frontend (Angular).

### Step 1 — Start the Backend API

Open a terminal in the project folder and run:

```bash
cd DocAI.API
dotnet run --launch-profile http
```

**Expected output:**
```
Now listening on: http://localhost:5115
Application started.
```

✅ The database is automatically created/migrated on first run.

📖 **API Documentation (Scalar UI):** http://localhost:5115/scalar/v1

### Step 2 — Start the Frontend

Open a **second terminal** and run:

```bash
cd DocAI.Angular
ng serve
```

**Expected output:**
```
➜  Local:   http://localhost:4200/
Watch mode enabled.
```

### Step 3 — Open the App

Open your browser and go to: **http://localhost:4200**

You will see the DocAI login page.

---

## 4. First-Time Setup: Register an Account

Since this is a fresh installation, no accounts exist yet. You must register first.

### How to Register

1. Open **http://localhost:4200**
2. You will see the Login page with two tabs: **Sign In** and **Register**
3. Click the **"Register"** tab
4. Fill in the form:

| Field | Description | Example |
|-------|-------------|---------|
| **Full Name** | Your full name with title | `Dr. Ahmed Hassan` |
| **Username** | Unique username (no spaces) | `dr.ahmed` |
| **Email** | Your work email | `ahmed@hospital.com` |
| **Department** | Your medical department | `Cardiology` |
| **Role** | Your role in the system | `Physician` |
| **Password** | Minimum 6 characters | `Hospital@2024` |

5. Click **"Create Account"**
6. You will be automatically logged in and redirected to the **Dashboard**

### Roles Explained

| Role | What they can do |
|------|-----------------|
| **Physician** | Create cases, generate audits, approve/reject |
| **Auditor** | View all cases and reports (read-only) |
| **Admin** | Full access to everything |

---

## 5. Login

1. Open **http://localhost:4200**
2. The **"Sign In"** tab is shown by default
3. Enter your **Email** and **Password**
4. Click **"Sign In"**
5. You are redirected to the **Dashboard**

---

## 6. The Dashboard

After login, you land on the Dashboard. Here's what you see:

```
┌─────────────────────────────────────────────────────────┐
│  Dashboard                            [+ New Case]       │
├──────────┬──────────┬──────────┬──────────┬─────────────┤
│  Total   │ Approved │ Pending  │ Rejected │  Avg Rate   │
│  Cases   │          │ Review   │          │             │
│    12    │    8     │    3     │    1     │   87.5%     │
├──────────┴──────────┴──────────┴──────────┴─────────────┤
│  Recent Cases                                            │
│  ┌──────────────┬──────────────┬──────────┬────────────┐ │
│  │ Patient Name │  Diagnosis   │  Status  │ Accept Rate│ │
│  ├──────────────┼──────────────┼──────────┼────────────┤ │
│  │ John Smith   │ Hypertension │ Approved │   92%      │ │
│  │ Sara Ali     │ Diabetes     │ Pending  │   78%      │ │
│  └──────────────┴──────────────┴──────────┴────────────┘ │
└─────────────────────────────────────────────────────────┘
```

### Dashboard Elements

**Stat Cards (top row):**
- 🔵 **Total Cases** — all cases you've created
- 🟢 **Approved** — cases physician has approved
- 🟠 **Pending Review** — cases waiting for physician action
- 🔴 **Rejected** — cases sent back for revision
- 🟣 **Avg Acceptance Rate** — average AI-estimated insurance acceptance %

**Recent Cases Table:**
- Shows your last 10 cases
- **Status badge colors:** 🟢 Approved · 🟠 PendingReview · 🔴 Rejected · ⚪ Draft
- **👁 Eye icon** → View the case details
- **📈 Chart icon** → Go to the AI Audit report

**Navigation Sidebar (left side):**
- 🏠 Dashboard
- ➕ New Case
- 📋 All Cases

---

## 7. Creating a Patient Case (Step-by-Step)

A patient case is the core record. It contains all clinical information the AI needs to audit.

### How to Create a Case

1. Click **"+ New Case"** button (Dashboard or sidebar)
2. You arrive at the **6-tab case entry form**

---

### Tab 1: Patient Info

Fill in basic patient demographics:

| Field | Required | Description | Example |
|-------|----------|-------------|---------|
| Patient ID | ✅ Yes | Hospital ID number | `P-20240418` |
| Patient Name | ✅ Yes | Full name | `Mohammed Al-Rashid` |
| Age | ✅ Yes | Age in years | `54` |
| Gender | ✅ Yes | Male / Female / Other | `Male` |
| Physician Name | No | Attending physician | `Dr. Ahmed Hassan` |
| Status | No | Case status | `Draft` |

---

### Tab 2: Chief Complaint

| Field | Required | Description | Example |
|-------|----------|-------------|---------|
| Chief Complaint | ✅ Yes | Why did the patient come? | `Chest pain for 2 days, radiating to left arm` |
| History of Present Illness (HoPI) | No (but strongly recommended) | Detailed story of the illness | `Patient is a 54-year-old male with known hypertension who presents with 2-day history of chest pain...` |
| Medications Prescribed | No | All medications ordered | `Aspirin 100mg daily, Atorvastatin 40mg` |

> 💡 **Tip:** The more detail you enter in HoPI, the better the AI audit score. Empty fields will be flagged as insurance risks.

---

### Tab 3: Examination

| Field | Required | Description | Example |
|-------|----------|-------------|---------|
| Physical Examination | No | Exam findings | `BP 150/95 mmHg, HR 88 bpm, Chest: clear to auscultation...` |
| Progress Note | No | Clinical observations | `Patient appears anxious, diaphoretic. ECG shows ST elevation in leads II, III, aVF...` |
| Provisional Diagnosis | No | Your working diagnosis | `Acute inferior STEMI` |

---

### Tab 4: Orders

| Field | Description | Example |
|-------|-------------|---------|
| Labs Requested | Blood tests ordered | `CBC, BMP, Troponin I, BNP, Lipid panel` |
| Imaging Requested | X-rays, CT, MRI, Echo | `Chest X-ray PA, 12-lead ECG, Echo` |
| Procedures Requested | Procedures ordered | `Urgent coronary angiography` |
| Medication Dispensed | Medications given | `Aspirin 300mg, Heparin 5000 units IV` |

---

### Tab 5: Results

Fill this tab **after you have results back** from labs/imaging:

| Field | Description | Example |
|-------|-------------|---------|
| Lab Results | Results of blood tests | `Troponin I: 2.5 ng/mL (HIGH), CBC: WBC 11.2...` |
| Imaging Results | Radiology/Echo findings | `Echo: EF 40%, inferior wall hypokinesia...` |

---

### Tab 6: History

| Field | Description | Example |
|-------|-------------|---------|
| Previous Visits | Last 10 encounters/episodes | `2024-01: Hypertension follow-up. BP 145/90. Medication adjusted...` |

---

### Saving the Case

You have two buttons at the top right:

| Button | What it does |
|--------|--------------|
| **💾 Save Draft** | Saves the case as "Draft" without running the AI. Use when you are still filling in information. |
| **⚡ Save & Generate Audit** | Saves AND immediately triggers the full AI audit (OpenAI + ICD-10 + RxNorm + LOINC). Takes 10–20 seconds. |

---

## 8. Generating the AI Audit Report

After saving a case, you can generate the AI audit at any time.

### How to Generate

**Option A — From the Case Form:**
Click **"⚡ Save & Generate Audit"** button

**Option B — From the Dashboard:**
Click the **📈 chart icon** next to any case

**Option C — From the Audit Page:**
If no report exists yet, you'll see a **"⚡ Generate AI Audit"** button

### What happens when you generate?

The system makes **6 API calls simultaneously** (in parallel):

```
Your Case Data
     │
     ├──► OpenAI GPT-4o ──────────────────────► All 7 audit sections
     ├──► NLM ICD-10 API ─────────────────────► Diagnosis codes
     ├──► NLM RxNorm API ─────────────────────► Medication codes  
     ├──► NLM LOINC API ──────────────────────► Lab test codes
     ├──► SNOMED CT (stub) ───────────────────► Clinical concepts
     └──► Amazon Comprehend Medical (stub) ───► Entity extraction
                    │
                    ▼
            Saved to Database
                    │
                    ▼
           Audit Report Displayed
```

**Time:** Typically 10–25 seconds depending on OpenAI response time.

---

## 9. Understanding the Audit Report

The audit report page has two sections: **Score Summary** at the top, and **Detailed Accordion** below.

### Score Summary Bar

```
┌──────────────┬─────────────────────┬────────────┬───────────────┬────────────────────┐
│ Documentation│ Clinical Consistency│  Care Plan │Insurance Risk │ Overall Acceptance │
│   85/100     │      90/100         │   78/100   │   25/100      │       87%          │
│    🟢        │        🟢           │    🟡      │    🟢         │      🟢            │
└──────────────┴─────────────────────┴────────────┴───────────────┴────────────────────┘
```

**Score Colors:**
- 🟢 **Green (≥70)** = Good
- 🟡 **Yellow (40–69)** = Needs attention
- 🔴 **Red (<40)** = Critical issue

> ⚠️ **Insurance Risk Score is INVERTED**: Higher = More Risky. A risk score of 80 = HIGH risk. A risk score of 10 = LOW risk.

---

### Accordion Sections (Click to expand each)

#### 📄 Section 1: Documentation Review
Shows whether all required clinical fields are filled:
- ✅ Chief Complaint present and detailed
- ✅ HoPI documented
- ⚠️ Physical Examination missing
- ⚠️ Progress Note vague

**Score 0–100.** Higher = more complete documentation.

---

#### ❤️ Section 2: Clinical Consistency Review
Checks if the diagnosis makes clinical sense given:
- Symptoms in the chief complaint
- Physical examination findings
- Lab and imaging results
- Medications prescribed

Example output:
> *"The diagnosis of Acute Inferior STEMI is consistent with the presenting symptoms of chest pain with radiation, ST elevation on ECG, and elevated troponin. Medications include Aspirin and Heparin which are appropriate. However, the beta-blocker therapy is not documented..."*

---

#### 🧭 Section 3: Care Plan Review
Evaluates whether your orders are clinically appropriate:
- Are the labs ordered sufficient for this diagnosis?
- Is imaging appropriate?
- Are any important tests missing?
- Are medications aligned with standard of care?

Example output:
> *"CBC and metabolic panel are appropriate. Troponin serial measurement recommended every 3–6 hours but only one value documented. Echo is appropriate. Missing: Coagulation studies before heparin therapy..."*

---

#### 🛡️ Section 4: Insurance Risk Flags
This is the most important section for preventing claim rejection.

The AI flags specific issues with ⚠️ symbols:

Example output:
> *⚠️ HIGH RISK: Physical examination is not documented. Most insurance providers require a documented physical exam for inpatient admission.*
>
> *⚠️ MEDIUM RISK: Diagnosis lacks ICD-10 specificity. "Chest pain" is too vague for STEMI claims. Document as "Acute ST-elevation myocardial infarction of inferior wall" (ICD-10: I21.19)*
>
> *⚠️ MEDIUM RISK: Procedures requested (coronary angiography) lack documented medical necessity in the progress note.*

**Risk Score 0–100.** Lower = safer for insurance. **Target: below 30.**

---

#### 💡 Section 5: Suggested Improvements (Editable)
The AI rewrites weak or missing documentation. **You can edit this text** before approving.

Example:
> *"Physical Examination: Patient is a 54-year-old male in moderate distress. Vital signs: BP 150/95 mmHg, HR 88 bpm, RR 18, O2 sat 96% on room air, Temperature 37.2°C. Cardiovascular: Regular rate and rhythm, no murmurs. Respiratory: Clear to auscultation bilaterally..."*

**How to use:** 
1. Read the suggested text
2. Modify it to match the actual clinical situation
3. Proceed to approve (your edits are saved)

---

#### ℹ️ Section 6: Final Summary & Rationale
A brief action list for the physician:

Example:
> *"SUMMARY: Documentation is 85% complete. Key action items:*
> *1. Add physical examination findings*
> *2. Document serial troponin measurements*
> *3. Add medical necessity statement for angiography*
> *4. Include coagulation studies order*
>
> *ACCEPTANCE RATIONALE: Current documentation would likely be accepted at 87% probability. The remaining 13% risk is due to missing physical examination and insufficient procedure justification. Completing the suggested improvements would raise acceptance to approximately 95%."*

---

#### 🏷️ Section 7: Coded Items
Shows codes auto-extracted from your documentation:

- **ICD-10 Codes** — Diagnosis codes (e.g., `I21.19 - Acute STEMI of inferior wall`)
- **RxNorm Codes** — Medication codes (e.g., `1191 - Aspirin`)
- **LOINC Codes** — Lab test codes (e.g., `10839-9 - Troponin I`)
- **SNOMED Codes** — Clinical concepts (stub — coming soon)

---

### Action Bar (Bottom)

After reviewing the report, choose an action:

| Button | Action | When to use |
|--------|--------|-------------|
| ✅ **Approve** | Accepts the case as-is | Documentation is complete and good |
| ✏️ **Approve with Edits** | Accepts with your edited improvements | You modified the suggested text |
| ❌ **Reject** | Sends case back for revision | Critical issues found |

---

## 10. Approval Workflow

The approval workflow page shows the full history of actions on a case.

### How to Access
- Click **"Approval Workflow"** button on the Audit Report page
- Or navigate from the Cases List → Approval icon

### Approval Form

If the case is in **PendingReview** status:

| Field | Description |
|-------|-------------|
| **Action** | Approved / Rejected / EditedAndApproved |
| **Comments** | Your notes for the record (e.g., "Added physical exam findings") |
| **Approved Improvements** | Paste the final improved documentation text here |

### Approval Timeline
Shows a chronological history of all actions:
```
📅 2024-04-18 16:05  Dr. Ahmed Hassan  →  Approved with Edits
   Comments: "Updated physical exam documentation"

📅 2024-04-18 15:30  Dr. Sara Ali      →  PendingReview  
   Comments: "Generated AI audit report"
```

### Case Status Flow

```
DRAFT → PendingReview → Approved
                     ↘ Rejected → (Edit Case) → PendingReview → Approved
```

---

## 11. Cases List (All Cases)

Access via the sidebar: **"📋 All Cases"**

### Features

- **Paginated table** showing all cases (20 per page)
- **Search** by patient name
- **Filter** by status (Draft / PendingReview / Approved / Rejected)
- **Columns:** Patient Name, Patient ID, Diagnosis, Status, Acceptance Rate, Physician, Date

### Action Buttons per row

| Icon | Action |
|------|--------|
| 👁 Eye | View case details |
| 📈 Chart | View / Generate audit report |
| ✅ Check | View approval workflow |

---

## 12. Complete End-to-End Workflow

Here is the full workflow from start to finish:

```
STEP 1: Register / Login
        ↓
STEP 2: Click "+ New Case" on Dashboard
        ↓
STEP 3: Fill all 6 tabs of the patient case form
        ├─ Tab 1: Patient Info (ID, Name, Age, Gender)
        ├─ Tab 2: Chief Complaint + HoPI + Medications
        ├─ Tab 3: Physical Exam + Progress Note + Diagnosis
        ├─ Tab 4: Labs/Imaging/Procedures Ordered
        ├─ Tab 5: Lab Results + Imaging Results
        └─ Tab 6: Previous Visits History
        ↓
STEP 4: Click "⚡ Save & Generate Audit"
        (Wait 10–25 seconds for AI to process)
        ↓
STEP 5: Review the Audit Report
        ├─ Check Score Summary (top)
        ├─ Read Documentation Review (Section 1)
        ├─ Read Clinical Consistency (Section 2)
        ├─ Read Care Plan Review (Section 3)
        ├─ Check Insurance Risk Flags ⚠️ (Section 4) ← Most important!
        ├─ Edit Suggested Improvements (Section 5) ← Modify as needed
        ├─ Read Final Summary (Section 6)
        └─ Note ICD-10/RxNorm/LOINC codes (Section 7)
        ↓
STEP 6: Take Action
        ├─ ✅ Approve (if documentation is complete)
        ├─ ✏️ Approve with Edits (if you edited improvements)
        └─ ❌ Reject (if major issues remain)
        ↓
STEP 7: Case is "Approved" → Ready for insurance submission
```

### Best Practices for High Acceptance Rate

1. **Never leave Chief Complaint empty** — it is the foundation of the claim
2. **Always document Physical Examination** — most payers require it
3. **Make HoPI detailed** — include onset, duration, character, severity, location, radiation, associated symptoms
4. **Document medical necessity** in the Progress Note for every ordered test/procedure
5. **Use specific diagnoses** — "Hypertensive urgency" instead of "high blood pressure"
6. **Match medications to diagnosis** — prescribed drugs should logically relate to the diagnosis
7. **Enter Lab Results when available** — the AI can validate clinical decisions against actual results
8. **Review Insurance Risk Flags section carefully** — every ⚠️ flag is a potential rejection reason

---

## 13. API Reference (for developers)

Base URL: `http://localhost:5115`

Interactive docs: `http://localhost:5115/scalar/v1`

### Authentication
All endpoints (except login/register) require JWT Bearer token:
```
Authorization: Bearer <your_token>
```

### Endpoints

#### Auth
```
POST /api/auth/register    Register new user
POST /api/auth/login       Login → returns JWT token
```

#### Patient Cases
```
POST   /api/patientcases           Create new case
GET    /api/patientcases           List cases (paginated)
GET    /api/patientcases/{id}      Get case + audit report
PUT    /api/patientcases/{id}      Update draft case
```

#### Audit
```
POST   /api/audit/{caseId}/generate    Generate AI audit report
GET    /api/audit/{caseId}             Get existing audit report
```

#### Approval
```
POST   /api/approval                   Submit approval action
GET    /api/approval/{caseId}          Get approval history
GET    /api/approval/dashboard/stats   Get dashboard statistics
```

### Example: Create a Case via API

```bash
curl -X POST http://localhost:5115/api/patientcases \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "P-001",
    "patientName": "Mohammed Al-Rashid",
    "patientAge": 54,
    "patientGender": "Male",
    "chiefComplaint": "Chest pain for 2 days",
    "hopi": "54-year-old male presenting with...",
    "provisionalDiagnosis": "Acute Inferior STEMI",
    "medicationsPrescribed": "Aspirin 300mg, Heparin 5000 units IV"
  }'
```

### Example: Generate Audit

```bash
curl -X POST http://localhost:5115/api/audit/{caseId}/generate \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 14. Troubleshooting

### ❌ "Cannot connect to database"
**Cause:** SQL Server not running or wrong connection string.

**Fix:**
1. Check SQL Server is running: open **Services** → find **SQL Server (SQLEXPRESS)** → Start it
2. Verify `appsettings.json` has `Server=localhost\\SQLEXPRESS`

---

### ❌ "OpenAI API error" or "401 Unauthorized from OpenAI"
**Cause:** Invalid or missing OpenAI API key.

**Fix:**
1. Get your key from: https://platform.openai.com/api-keys
2. Paste it in `appsettings.json` under `OpenAI:ApiKey`
3. Ensure you have billing set up on your OpenAI account
4. Restart the backend (`dotnet run`)

---

### ❌ "Port 5115 already in use"
**Cause:** A previous backend instance is still running.

**Fix:**
```powershell
# Find and kill the process
netstat -ano | findstr :5115
# Note the PID from the last column, then:
taskkill /PID <PID> /F
```

---

### ❌ Frontend shows blank page / routing error
**Fix:** Clear browser cache and refresh. Or open: `http://localhost:4200/login`

---

### ❌ "Invalid email or password" on first login
**Cause:** No account exists yet.

**Fix:** Click the **"Register"** tab on the login page to create your first account.

---

### ❌ Audit generation takes too long / times out
**Cause:** OpenAI API latency.

**Fix:** 
- Wait up to 30 seconds
- Check your OpenAI account has sufficient credits
- Check internet connection

---

## 15. Roles & Permissions

| Action | Physician | Auditor | Admin |
|--------|-----------|---------|-------|
| Create patient case | ✅ | ❌ | ✅ |
| Edit draft case | ✅ | ❌ | ✅ |
| Generate AI audit | ✅ | ❌ | ✅ |
| View audit reports | ✅ | ✅ | ✅ |
| Approve / Reject cases | ✅ | ❌ | ✅ |
| View all cases | ✅ | ✅ | ✅ |
| View dashboard stats | ✅ | ✅ | ✅ |

---

## Quick Reference Card

```
┌─────────────────────────────────────────────┐
│           DocAI Quick Reference             │
├─────────────────────────────────────────────┤
│ App URL:      http://localhost:4200          │
│ API URL:      http://localhost:5115          │
│ API Docs:     http://localhost:5115/scalar/v1│
├─────────────────────────────────────────────┤
│ WORKFLOW:                                    │
│  1. Login / Register                         │
│  2. New Case → Fill 6 tabs                  │
│  3. Save & Generate Audit                   │
│  4. Review 7 sections                       │
│  5. Fix Insurance Risk Flags ⚠️             │
│  6. Edit Suggested Improvements if needed   │
│  7. Approve / Reject                        │
├─────────────────────────────────────────────┤
│ SCORE GUIDE:                                 │
│  🟢 Green  ≥70   = Good                     │
│  🟡 Yellow 40-69 = Needs attention          │
│  🔴 Red    <40   = Critical                 │
│                                             │
│  ⚠️ Insurance Risk: LOWER is better         │
│  ✅ Acceptance Rate: HIGHER is better        │
└─────────────────────────────────────────────┘
```

---

*DocAI v1.0 — Built with .NET 10, Angular 21, OpenAI GPT-4o*
*APIs: ICD-10 · RxNorm · LOINC · SNOMED (stub) · Amazon Comprehend (stub) · UMLS (stub)*
