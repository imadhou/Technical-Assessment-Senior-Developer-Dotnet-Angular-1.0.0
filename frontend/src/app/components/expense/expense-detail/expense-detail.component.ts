// expense-detail.component.ts
import { Component, OnInit, signal, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ExpenseService, ReceiptService } from '../../../services/services.index';
import { Expense, ExpenseCommand } from '../../../models/model.index';
import { lastValueFrom } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-expense-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './expense-detail.component.html',
  styleUrl: './expense-detail.component.scss'
})
export class ExpenseDetailComponent{
  private expenseService = inject(ExpenseService);
  private receiptService = inject(ReceiptService);

  @Input() expense!: Expense;
  @Output() close = new EventEmitter<void>();
  @Input() set editMode(value: boolean) {
    if(value) {
      this.enterEditMode();
    }
  }
  @Output() expenseUpdated = new EventEmitter<Expense>();

  // Mode and state signals
  isEditMode = signal(false);
  loading = signal(false);
  errorMessage = signal('');
  errorList = signal<string[]>([]);

  // Form data
  editForm = signal({
    description: '',
    amount: 0,
    date: '',
    expenseCategoryId: '',
    userId: ''
  });

  // File upload
  selectedFile: File | null = null;

  ngOnInit() {
    this.initializeForm();
  }

  initializeForm() {
    if (this.expense) {
      this.editForm.set({
        description: this.expense.description || '',
        amount: this.expense.amount,
        date: this.expense.date.split('T')[0], // Format for date input
        expenseCategoryId: this.expense.expenseCategoryId,
        userId: this.expense.userId
      });
    }
  }

  // Mode switching
  enterEditMode() {
    this.isEditMode.set(true);
    this.errorMessage.set('');
    this.errorList.set([]);
  }

  cancelEdit() {
    this.isEditMode.set(false);
    this.initializeForm();
    this.selectedFile = null;
    this.errorMessage.set('');
    this.errorList.set([]);
  }

  // File handling
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      // Validate file type and size
      const validTypes = ['image/jpeg', 'image/png', 'image/jpg', 'application/pdf'];
      const maxSize = 5 * 1024 * 1024; // 5MB

      if (!validTypes.includes(file.type)) {
        this.errorMessage.set('Please select a valid file type (JPEG, PNG, PDF)');
        return;
      }

      if (file.size > maxSize) {
        this.errorMessage.set('File size must be less than 5MB');
        return;
      }

      this.selectedFile = file;
      this.errorMessage.set('');
    }
  }

  removeReceipt() {
    if (confirm('Are you sure you want to remove the receipt?')) {
      this.loading.set(true);
      this.receiptService.deleteReceipt(this.expense.id).subscribe({
        next: () => {
          // Update local expense state
          this.expense = {
            ...this.expense,
            receiptUrl: null,
            receiptFileName: null,
            receiptFileSize: null,
            receiptUploadedAt: null
          };
          this.loading.set(false);
        },
        error: (error) => {
          this.handleError(error);
          this.loading.set(false);
        }
      });
    }
  }

  // Save expense
  async saveExpense() {
    this.loading.set(true);
    this.errorMessage.set('');
    this.errorList.set([]);

    try {
      const formData = this.editForm();
      const expenseCommand: ExpenseCommand = {
        description: formData.description,
        amount: formData.amount,
        expenseCategoryId: formData.expenseCategoryId,
        date: new Date(formData.date).toISOString(),
        userId: formData.userId
      };

      // First update the expense
      const updatedExpense = await lastValueFrom(this.expenseService.updateExpense(this.expense.id, expenseCommand));

      // Then upload receipt if a new file was selected
      if (this.selectedFile) {
        await this.receiptService.uploadReceipt(this.selectedFile, this.expense.id).toPromise();
        // Refresh expense to get updated receipt info
        const refreshedExpense = await lastValueFrom(this.expenseService.getExpenseById(this.expense.id));
        this.expenseUpdated.emit(refreshedExpense);
        this.expense = refreshedExpense;
      } else {
        this.expenseUpdated.emit(updatedExpense);
        this.expense = updatedExpense;
      }

      this.isEditMode.set(false);
      this.selectedFile = null;

    } catch (error: any) {
      this.handleError(error);
    } finally {
      this.loading.set(false);
    }
  }

  // Error handling (same logic as login)
  private handleError(error: any) {
    if (error.error?.errors) {
      // Handle ValidationProblemDetails (array of errors)
      const errors: string[] = [];
      Object.keys(error.error.errors).forEach(key => {
        errors.push(...error.error.errors[key]);
      });
      this.errorList.set(errors);
      this.errorMessage.set('Please correct the validation errors below.');
    } else {
      // Handle ProblemDetails (single message)
      this.errorMessage.set(error.error?.message || error.message || 'An error occurred while updating the expense.');
      this.errorList.set([]);
    }
  }

  // Utility methods (same as before)
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatDateTime(dateString: string): string {
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusBadgeClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'approved':
        return 'status-badge status-approved';
      case 'rejected':
        return 'status-badge status-rejected';
      case 'pending':
      default:
        return 'status-badge status-pending';
    }
  }

  getFileSize(bytes: number | null): string {
    if (!bytes) return 'Unknown';

    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    if (bytes === 0) return '0 Bytes';

    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    return Math.round(bytes / Math.pow(1024, i) * 100) / 100 + ' ' + sizes[i];
  }

  downloadReceipt() {
    this.receiptService.downloadReceipt(this.expense.id, this.expense.receiptFileName || 'receipt');
  }

  getUserInitials(): string {
    if (!this.expense?.user) return '??';

    const firstName = this.expense.user.firstName || '';
    const lastName = this.expense.user.lastName || '';

    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase() || 'U';
  }

  onClose() {
    this.close.emit();
  }
}
