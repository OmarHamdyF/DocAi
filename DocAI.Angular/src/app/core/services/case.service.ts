import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PatientCase } from '../../shared/models/patient-case.model';
import { PaginatedResponse } from '../../shared/models/api-response.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CaseService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/patientcases`;

  getCases(page: number = 1, pageSize: number = 20): Observable<PaginatedResponse<PatientCase>> {
    const params = new HttpParams().set('page', page.toString()).set('pageSize', pageSize.toString());
    return this.http.get<PaginatedResponse<PatientCase>>(this.apiUrl, { params });
  }

  getCaseById(id: string): Observable<PatientCase> {
    return this.http.get<PatientCase>(`${this.apiUrl}/${id}`);
  }

  createCase(caseData: PatientCase): Observable<PatientCase> {
    return this.http.post<PatientCase>(this.apiUrl, caseData);
  }

  updateCase(id: string, caseData: PatientCase): Observable<PatientCase> {
    return this.http.put<PatientCase>(`${this.apiUrl}/${id}`, caseData);
  }
}
