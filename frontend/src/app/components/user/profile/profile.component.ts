import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/services.index';
import { User } from '../../../models/model.index';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  private authService = inject(AuthService);

  loading = signal(true);
  user = signal<User | null>(null);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadUserProfile();
  }

  loadUserProfile(): void {
    this.loading.set(true);
    this.authService.getCurrentUser().subscribe({
      next: (user) => {
        this.user.set(user);
        this.loading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Failed to load profile data');
        this.loading.set(false);
      }
    });
  }

  // Helper method to get user initials for avatar
  getUserInitials(): string {
    const user = this.user();
    if (!user) return 'U';

    const first = user.firstName?.charAt(0) || '';
    const last = user.lastName?.charAt(0) || '';
    return (first + last).toUpperCase() || 'U';
  }

  // Format date for display
  formatDate(dateString: string|null|undefined): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getRoleBadgeClass(roleName: string | null | undefined): string {
    if (!roleName) return 'bg-gray-100 text-gray-800';

    switch (roleName.toLowerCase()) {
      case 'admin':
        return 'bg-red-100 text-red-800';
      case 'manager':
        return 'bg-blue-100 text-blue-800';
      case 'user':
        return 'bg-green-100 text-green-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }
}
