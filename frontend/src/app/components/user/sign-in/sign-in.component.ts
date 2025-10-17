import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/services.index';
import { LoginCommand, ProblemDetails} from '../../../models/model.index';

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss']
})
export class SignInComponent {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private router = inject(Router);

  loading = signal(false);
  errorList = signal<string[]>([]);
  errorMessage = signal(''); // for non-validation errors

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading.set(true);
    this.errorList.set([]);
    this.errorMessage.set('');

    const loginCommand: LoginCommand = {
      email: this.loginForm.value.email!,
      password: this.loginForm.value.password!
    };

    this.authService.login(loginCommand).subscribe({
      next: (response) => {
        this.loading.set(false);
        // Navigate to expenses or dashboard
        this.router.navigate(['/expenses']);
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
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }
}
