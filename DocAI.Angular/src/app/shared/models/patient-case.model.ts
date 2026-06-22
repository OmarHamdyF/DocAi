export interface PatientCase {
  id?: string;
  patientId: string;
  patientName: string;
  patientAge: number;
  patientGender: string;
  chiefComplaint: string;
  hopi: string;
  physicalExam: string;
  progressNote: string;
  provisionalDiagnosis: string;
  medicationsPrescribed: string;
  labsRequested: string;
  imagingRequested: string;
  proceduresRequested: string;
  labResults: string;
  imagingResults: string;
  medicationDispensed: string;
  previousVisits: string;
  status: string;
  createdAt?: string;
  physicianName?: string;
}

export interface CodedItem {
  code: string;
  display: string;
  system: string;
}

export interface RecommendedItem {
  name: string;
  reason: string;
  urgency: 'Routine' | 'Urgent' | 'Stat';
}

export interface AuditReport {
  id: string;
  patientCaseId: string;
  documentationReview: string;
  documentationScore: number;
  clinicalConsistencyReview: string;
  clinicalConsistencyScore: number;
  carePlanReview: string;
  carePlanScore: number;
  insuranceRiskFlags: string;
  insuranceRiskScore: number;
  suggestedImprovements: string;
  finalSummary: string;
  overallAcceptanceRate: number;
  acceptanceRationale: string;
  icd10Codes: CodedItem[];
  rxNormCodes: CodedItem[];
  loincCodes: CodedItem[];
  snomedCodes: CodedItem[];
  recommendedLabs: RecommendedItem[];
  recommendedImaging: RecommendedItem[];
  recommendedProcedures: RecommendedItem[];
  recommendedConsultations: RecommendedItem[];
  generatedAt: string;
  modelUsed: string;
}

export interface ApprovalRecord {
  id?: string;
  patientCaseId: string;
  action: string;
  comments: string;
  approvedImprovements: string;
  createdAt?: string;
  createdBy?: string;
}

export interface AuthResponse {
  token: string;
  fullName: string;
  role: string;
  username: string;
  email: string;
  expiresAt: string;
}

export interface DashboardStats {
  totalCases: number;
  approvedCases: number;
  pendingReviewCases: number;
  rejectedCases: number;
  avgAcceptanceRate: number;
}
