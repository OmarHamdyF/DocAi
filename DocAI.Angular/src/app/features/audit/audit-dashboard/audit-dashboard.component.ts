import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { AccordionModule } from 'primeng/accordion';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ChipModule } from 'primeng/chip';
import { MessageService } from 'primeng/api';
import { CaseService } from '../../../core/services/case.service';
import { AuditService } from '../../../core/services/audit.service';
import { ApprovalService } from '../../../core/services/approval.service';
import { PatientCase, AuditReport, ApprovalRecord, RecommendedItem } from '../../../shared/models/patient-case.model';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-audit-dashboard',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    ButtonModule, CardModule, AccordionModule, TagModule, TextareaModule,
    ProgressSpinnerModule, ChipModule, FormsModule
  ],
  templateUrl: './audit-dashboard.component.html',
  styleUrl: './audit-dashboard.component.scss'
})
export class AuditDashboardComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private caseService = inject(CaseService);
  private auditService = inject(AuditService);
  private approvalService = inject(ApprovalService);
  private messageService = inject(MessageService);
  private fb = inject(FormBuilder);

  loading = signal(false);
  generating = signal(false);
  approving = signal(false);
  caseId = signal<string>('');
  patientCase = signal<PatientCase | null>(null);
  auditReport = signal<AuditReport | null>(null);

  editedImprovements = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.caseId.set(id);
      this.loadData(id);
    }
  }

  loadData(id: string): void {
    this.loading.set(true);

    this.caseService.getCaseById(id).pipe(
      catchError(() => of(null))
    ).subscribe(data => {
      if (data) this.patientCase.set(data);
    });

    this.auditService.getAudit(id).pipe(
      catchError(() => of(null))
    ).subscribe(data => {
      this.loading.set(false);
      if (data) {
        this.auditReport.set(data);
        this.editedImprovements = data.suggestedImprovements;
      }
    });
  }

  generateAudit(): void {
    this.generating.set(true);
    this.auditService.generateAudit(this.caseId()).pipe(
      catchError(err => {
        this.generating.set(false);
        const detail = err.error?.message || err.error?.detail || 'Failed to generate audit report';
        this.messageService.add({ severity: 'error', summary: 'Audit Error', detail, life: 8000 });
        return of(null);
      })
    ).subscribe(data => {
      this.generating.set(false);
      if (data) {
        this.auditReport.set(data);
        this.editedImprovements = data.suggestedImprovements;
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Audit report generated successfully' });
      }
    });
  }

  submitApproval(action: string): void {
    this.approving.set(true);
    const record: ApprovalRecord = {
      patientCaseId: this.caseId(),
      action,
      comments: '',
      approvedImprovements: this.editedImprovements
    };

    this.approvalService.submitApproval(record).pipe(
      catchError(err => {
        this.approving.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to submit approval' });
        return of(null);
      })
    ).subscribe(data => {
      this.approving.set(false);
      if (data) {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: `Case ${action} successfully` });
        this.router.navigate(['/approval', this.caseId()]);
      }
    });
  }

  getUrgencySeverity(urgency: string): 'success' | 'info' | 'warn' | 'danger' {
    if (urgency === 'Stat')   return 'danger';
    if (urgency === 'Urgent') return 'warn';
    return 'info';
  }

  getScoreSeverity(score: number): 'success' | 'info' | 'warn' | 'danger' {
    if (score >= 80) return 'success';
    if (score >= 60) return 'info';
    if (score >= 40) return 'warn';
    return 'danger';
  }

  viewCase(): void {
    this.router.navigate(['/cases', this.caseId(), 'edit']);
  }

  goToApproval(): void {
    this.router.navigate(['/approval', this.caseId()]);
  }
}
