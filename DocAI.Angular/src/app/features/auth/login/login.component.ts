import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { PasswordModule } from 'primeng/password';
import { MessageModule } from 'primeng/message';
import { SelectModule } from 'primeng/select';
import { DividerModule } from 'primeng/divider';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CardModule, InputTextModule,
    ButtonModule, PasswordModule, MessageModule, SelectModule, DividerModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  errorMessage = signal('');
  isRegisterMode = signal(false);

  roles = [
    { label: 'Physician', value: 'Physician' },
    { label: 'Auditor', value: 'Auditor' },
    { label: 'Admin', value: 'Admin' }
  ];

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  registerForm = this.fb.group({
    fullName:   ['', [Validators.required, Validators.minLength(3)]],
    username:   ['', [Validators.required, Validators.minLength(3)]],
    email:      ['', [Validators.required, Validators.email]],
    department: ['', Validators.required],
    role:       ['Physician', Validators.required],
    password:   ['', [Validators.required, Validators.minLength(6)]]
  });

  toggleMode(): void {
    this.isRegisterMode.update(v => !v);
    this.errorMessage.set('');
    this.loginForm.reset();
    this.registerForm.reset({ role: 'Physician' });
  }

  onLogin(): void {
    if (this.loginForm.invalid) return;
    this.loading.set(true);
    this.errorMessage.set('');
    const { email, password } = this.loginForm.value;
    this.authService.login(email!, password!).subscribe({
      next: () => { this.loading.set(false); this.router.navigate(['/dashboard']); },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Invalid email or password.');
      }
    });
  }

  onRegister(): void {
    if (this.registerForm.invalid) return;
    this.loading.set(true);
    this.errorMessage.set('');
    const v = this.registerForm.value;
    this.authService.register({
      username:   v.username!,
      email:      v.email!,
      password:   v.password!,
      fullName:   v.fullName!,
      department: v.department!,
      role:       v.role!
    }).subscribe({
      next: () => { this.loading.set(false); this.router.navigate(['/dashboard']); },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Registration failed. Please try again.');
      }
    });
  }
}
