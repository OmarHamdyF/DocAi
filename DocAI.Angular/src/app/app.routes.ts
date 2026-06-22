import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: '',
    loadComponent: () => import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'cases',
        loadComponent: () => import('./features/cases-list/cases-list.component').then(m => m.CasesListComponent)
      },
      {
        path: 'cases/new',
        loadComponent: () => import('./features/case-entry/case-form/case-form.component').then(m => m.CaseFormComponent)
      },
      {
        path: 'cases/:id/edit',
        loadComponent: () => import('./features/case-entry/case-form/case-form.component').then(m => m.CaseFormComponent)
      },
      {
        path: 'audit/:id',
        loadComponent: () => import('./features/audit/audit-dashboard/audit-dashboard.component').then(m => m.AuditDashboardComponent)
      },
      {
        path: 'approval/:id',
        loadComponent: () => import('./features/approval/approval-workflow/approval-workflow.component').then(m => m.ApprovalWorkflowComponent)
      }
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];
