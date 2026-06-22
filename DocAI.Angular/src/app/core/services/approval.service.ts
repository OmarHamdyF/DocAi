import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApprovalRecord, DashboardStats } from '../../shared/models/patient-case.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ApprovalService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/approval`;

  submitApproval(record: ApprovalRecord): Observable<ApprovalRecord> {
    return this.http.post<ApprovalRecord>(this.apiUrl, record);
  }

  getHistory(caseId: string): Observable<ApprovalRecord[]> {
    return this.http.get<ApprovalRecord[]>(`${this.apiUrl}/${caseId}`);
  }

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard/stats`);
  }
}
