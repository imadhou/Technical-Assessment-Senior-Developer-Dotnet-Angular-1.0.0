import { Component, inject } from '@angular/core';
import { RouterOutlet, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/services.index';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterLink],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'App';
  private authService = inject(AuthService);
  private router = inject(Router);

  mobileMenuOpen = false;
  userMenuOpen = false;
  currentYear = new Date().getFullYear();

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen = !this.mobileMenuOpen;
    // Close user menu when mobile menu opens
    if (this.mobileMenuOpen) {
      this.userMenuOpen = false;
    }
  }

  toggleUserMenu(): void {
    this.userMenuOpen = !this.userMenuOpen;
    // Close mobile menu when user menu opens
    if (this.userMenuOpen) {
      this.mobileMenuOpen = false;
    }
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen = false;
  }

  logout(): void {
    this.authService.logout();
    this.userMenuOpen = false;
    this.router.navigate(['/auth/sign-in']);
  }

  // Close dropdowns when clicking outside
  onDocumentClick(event: Event): void {
    // This would need to be implemented with a directive for proper outside click detection
    // For now, we'll handle it in the template with simple toggles
  }
}
