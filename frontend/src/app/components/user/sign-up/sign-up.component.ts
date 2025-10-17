import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/services.index';
import { ProblemDetails, RegisterCommand } from '../../../models/model.index';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  loading = signal(false);
  errorMessage = signal('');
  errorList = signal<string[]>([]);

  registerForm = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  get firstName() { return this.registerForm.get('firstName'); }
  get lastName() { return this.registerForm.get('lastName'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.errorList.set([]);

    const registerCommand: RegisterCommand = {
      firstName: this.registerForm.value.firstName!,
      lastName: this.registerForm.value.lastName!,
      email: this.registerForm.value.email!,
      password: this.registerForm.value.password!
    };

    this.authService.register(registerCommand).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (error: ProblemDetails) => {
        this.loading.set(false);

        // Check if it's a ValidationProblemDetails inside details
        if (error.details?.errors) {
          const errors: string[] = [];
          if (Array.isArray(error.details.errors)) {
            errors.push(...error.details.errors.map(e => e.errorMessage));
            this.errorList.set(errors);
            this.errorMessage.set('');
          }
        }
        // Else just show the general message
        else if (!error.details?.errors && (error.detail || error.title || error.details?.message)) {
          this.errorMessage.set(
            error.detail || error.title || error.details?.message || 'An error occurred.'
          );
          this.errorList.set([]);
        }
        // Fallback
        else {
          this.errorMessage.set('Registration failed. Please try again.');
          this.errorList.set([]);
        }
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });
  }
}
