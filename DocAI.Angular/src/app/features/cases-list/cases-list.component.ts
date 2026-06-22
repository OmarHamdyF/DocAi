import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { CaseService } from '../../core/services/case.service';
import { PatientCase } from '../../shared/models/patient-case.model';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-cases-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, TableModule, TagModule, InputTextModule, SelectModule, ProgressSpinnerModule],
  templateUrl: './cases-list.component.html',
  styleUrl: './cases-list.component.scss'
})
export class CasesListComponent implements OnInit {
  private caseService = inject(CaseService);
  private messageService = inject(MessageService);
  private router = inject(Router);

  loading = signal(false);
  cases = signal<PatientCase[]>([]);
  total = signal(0);
  page = signal(1);
  pageSize = 20;
  searchTerm = '';
  selectedStatus = '';

  statusOptions = [
    { label: 'All Statuses', value: '' },
    { label: 'Draft', value: 'Draft' },
    { label: 'In Review', value: 'InReview' },
    { label: 'Pending Review', value: 'PendingReview' },
    { label: 'Approved', value: 'Approved' },
    { label: 'Rejected', value: 'Rejected' }
  ];

  ngOnInit(): void {
    this.loadCases();
  }

  loadCases(): void {
    this.loading.set(true);
    this.caseService.getCases(this.page(), this.pageSize).pipe(
      catchError(err => {
        this.loading.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load cases' });
        return of(null);
      })
    ).subscribe(data => {
      this.loading.set(false);
      if (data) {
        this.cases.set(data.items);
        this.total.set(data.total);
      }
    });
  }

  onPageChange(event: any): void {
    this.page.set(Math.floor(event.first / event.rows) + 1);
    this.loadCases();
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

  get filteredCases(): PatientCase[] {
    return this.cases().filter(c => {
      const matchesSearch = !this.searchTerm ||
        c.patientName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        c.patientId?.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = !this.selectedStatus || c.status === this.selectedStatus;
      return matchesSearch && matchesStatus;
    });
  }

  viewCase(id: string): void {
    this.router.navigate(['/cases', id, 'edit']);
  }

  viewAudit(id: string): void {
    this.router.navigate(['/audit', id]);
  }

  viewApproval(id: string): void {
    this.router.navigate(['/approval', id]);
  }

  newCase(): void {
    this.router.navigate(['/cases/new']);
  }
}
