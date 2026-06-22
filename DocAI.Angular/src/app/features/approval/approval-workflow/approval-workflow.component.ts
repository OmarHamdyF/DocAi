import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { TimelineModule } from 'primeng/timeline';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { CaseService } from '../../../core/services/case.service';
import { ApprovalService } from '../../../core/services/approval.service';
import { PatientCase, ApprovalRecord } from '../../../shared/models/patient-case.model';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-approval-workflow',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, FormsModule,
    ButtonModule, CardModule, SelectModule, TextareaModule, TimelineModule, TagModule, ProgressSpinnerModule
  ],
  templateUrl: './approval-workflow.component.html',
  styleUrl: './approval-workflow.component.scss'
})
export class ApprovalWorkflowComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private caseService = inject(CaseService);
  private approvalService = inject(ApprovalService);
  private messageService = inject(MessageService);
  private fb = inject(FormBuilder);

  loading = signal(false);
  submitting = signal(false);
  caseId = signal<string>('');
  patientCase = signal<PatientCase | null>(null);
  approvalHistory = signal<ApprovalRecord[]>([]);
  showForm = signal(false);

  actionOptions = [
    { label: 'Approve', value: 'Approve' },
    { label: 'Approve with Edits', value: 'ApproveWithEdits' },
    { label: 'Reject', value: 'Reject' }
  ];

  approvalForm = this.fb.group({
    action: ['', Validators.required],
    comments: ['', Validators.required],
    approvedImprovements: ['']
  });

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

    this.approvalService.getHistory(id).pipe(
      catchError(() => of([]))
    ).subscribe(data => {
      this.loading.set(false);
      const history = data || [];
      this.approvalHistory.set(history);
      const last = history[history.length - 1];
      this.showForm.set(!last || last.action === 'PendingReview');
    });
  }

  submitApproval(): void {
    if (this.approvalForm.invalid) {
      this.approvalForm.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    const record: ApprovalRecord = {
      patientCaseId: this.caseId(),
      action: this.approvalForm.value.action!,
      comments: this.approvalForm.value.comments!,
      approvedImprovements: this.approvalForm.value.approvedImprovements || ''
    };

    this.approvalService.submitApproval(record).pipe(
      catchError(err => {
        this.submitting.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to submit approval' });
        return of(null);
      })
    ).subscribe(data => {
      this.submitting.set(false);
      if (data) {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Approval submitted successfully' });
        this.loadData(this.caseId());
        this.approvalForm.reset();
      }
    });
  }

  getActionSeverity(action: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    const map: Record<string, 'success' | 'info' | 'warn' | 'danger' | 'secondary'> = {
      'Approve': 'success',
      'ApproveWithEdits': 'info',
      'Reject': 'danger',
      'PendingReview': 'warn'
    };
    return map[action] ?? 'secondary';
  }

  viewAudit(): void {
    this.router.navigate(['/audit', this.caseId()]);
  }

  viewCase(): void {
    this.router.navigate(['/cases', this.caseId(), 'edit']);
  }
}
