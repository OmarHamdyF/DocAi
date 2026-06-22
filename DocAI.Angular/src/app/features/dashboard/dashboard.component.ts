import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { CaseService } from '../../core/services/case.service';
import { ApprovalService } from '../../core/services/approval.service';
import { PatientCase, DashboardStats } from '../../shared/models/patient-case.model';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule, TableModule, TagModule, ProgressSpinnerModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private caseService = inject(CaseService);
  private approvalService = inject(ApprovalService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  loading = signal(false);
  stats = signal<DashboardStats>({
    totalCases: 0,
    approvedCases: 0,
    pendingReviewCases: 0,
    rejectedCases: 0,
    avgAcceptanceRate: 0
  });
  recentCases = signal<PatientCase[]>([]);

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);

    this.approvalService.getDashboardStats().pipe(
      catchError(() => of(null))
    ).subscribe(data => {
      if (data) this.stats.set(data);
    });

    this.caseService.getCases(1, 10).pipe(
      catchError(() => of(null))
    ).subscribe(data => {
      this.loading.set(false);
      if (data) this.recentCases.set(data.items);
    });
  }

  getStatusSeverity(status: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    const map: Record<string, 'success' | 'info' | 'warn' | 'danger' | 'secondary'> = {
      'Approved': 'success',
      'Rejected': 'danger',
      'PendingReview': 'warn',
      'Draft': 'secondary',
      'InReview': 'info'
    };
    return map[status] ?? 'secondary';
  }

  viewCase(id: string): void {
    this.router.navigate(['/cases', id, 'edit']);
  }

  viewAudit(id: string): void {
    this.router.navigate(['/audit', id]);
  }

  newCase(): void {
    this.router.navigate(['/cases/new']);
  }
}
