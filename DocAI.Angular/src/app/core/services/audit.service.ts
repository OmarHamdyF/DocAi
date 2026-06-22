import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuditReport } from '../../shared/models/patient-case.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuditService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/audit`;

  generateAudit(caseId: string): Observable<AuditReport> {
    return this.http.post<AuditReport>(`${this.apiUrl}/${caseId}/generate`, {});
  }

  getAudit(caseId: string): Observable<AuditReport> {
    return this.http.get<AuditReport>(`${this.apiUrl}/${caseId}`);
  }
}
