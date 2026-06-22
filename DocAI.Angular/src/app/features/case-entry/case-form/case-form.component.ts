import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { CaseService } from '../../../core/services/case.service';
import { PatientCase } from '../../../shared/models/patient-case.model';
import { catchError, of } from 'rxjs';

@Component({
  selector: 'app-case-form',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule,
    ButtonModule, InputTextModule, TextareaModule, SelectModule, CardModule, TabsModule, ProgressSpinnerModule
  ],
  templateUrl: './case-form.component.html',
  styleUrl: './case-form.component.scss'
})
export class CaseFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private caseService = inject(CaseService);
  private messageService = inject(MessageService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = signal(false);
  saving = signal(false);
  caseId = signal<string | null>(null);

  genderOptions = [
    { label: 'Male', value: 'Male' },
    { label: 'Female', value: 'Female' },
    { label: 'Other', value: 'Other' }
  ];

  statusOptions = [
    { label: 'Draft', value: 'Draft' },
    { label: 'In Review', value: 'InReview' },
    { label: 'Pending Review', value: 'PendingReview' },
    { label: 'Approved', value: 'Approved' },
    { label: 'Rejected', value: 'Rejected' }
  ];

  caseForm = this.fb.group({
    patientId: ['', Validators.required],
    patientName: ['', Validators.required],
    patientAge: [null as number | null, [Validators.required, Validators.min(0), Validators.max(150)]],
    patientGender: ['', Validators.required],
    physicianName: [''],
    status: ['Draft'],
    chiefComplaint: ['', Validators.required],
    hopi: [''],
    physicalExam: [''],
    progressNote: [''],
    provisionalDiagnosis: [''],
    medicationsPrescribed: [''],
    labsRequested: [''],
    imagingRequested: [''],
    proceduresRequested: [''],
    labResults: [''],
    imagingResults: [''],
    medicationDispensed: [''],
    previousVisits: ['']
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.caseId.set(id);
      this.loadCase(id);
    }
  }

  loadCase(id: string): void {
    this.loading.set(true);
    this.caseService.getCaseById(id).pipe(
      catchError(err => {
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to load case' });
        return of(null);
      })
    ).subscribe(data => {
      this.loading.set(false);
      if (data) {
        this.caseForm.patchValue(data as any);
      }
    });
  }

  saveDraft(): void {
    this.caseForm.patchValue({ status: 'Draft' });
    this.save(false);
  }

  saveAndGenerateAudit(): void {
    this.caseForm.patchValue({ status: 'PendingReview' });
    this.save(true);
  }

  private save(navigateToAudit: boolean): void {
    if (this.caseForm.invalid) {
      this.caseForm.markAllAsTouched();
      this.messageService.add({ severity: 'warn', summary: 'Validation', detail: 'Please fill all required fields' });
      return;
    }

    this.saving.set(true);
    const formValue = this.caseForm.value as PatientCase;

    const operation = this.caseId()
      ? this.caseService.updateCase(this.caseId()!, formValue)
      : this.caseService.createCase(formValue);

    operation.pipe(
      catchError(err => {
        this.saving.set(false);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to save case' });
        return of(null);
      })
    ).subscribe(saved => {
      this.saving.set(false);
      if (saved) {
        this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Case saved successfully' });
        if (navigateToAudit && saved.id) {
          this.router.navigate(['/audit', saved.id]);
        } else if (saved.id) {
          this.caseId.set(saved.id);
          this.router.navigate(['/cases', saved.id, 'edit']);
        }
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/cases']);
  }
}
