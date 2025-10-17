// expense-list.component.ts
import { Component, OnInit, signal, computed, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Expense, ExpenseCollection, ExpenseQueryParams } from '../../../models/model.index';
import { ExpenseService } from '../../../services/services.index';
import { lastValueFrom } from 'rxjs';
import { ExpenseDetailComponent } from '../expense-detail/expense-detail.component';

interface ExpenseFilter {
  description: string;
  fromDate: string;
  toDate: string;
  minAmount: number | null;
  maxAmount: number | null;
  expenseCategoryId: string;
  status: string;
}

interface SortConfig {
  field: string;
  direction: 'asc' | 'desc';
}

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ExpenseDetailComponent],
  templateUrl: './expense-list.component.html',
  styleUrl: './expense-list.component.scss'
})
export class ExpenseListComponent implements OnInit, OnDestroy {
  private expenseService = inject(ExpenseService);

  private debounceTimer: any;
  private debouncedFilterChange(): void {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => {
      this.onFilterChange();
    }, 300); // 300ms debounce
  }
  // State signals
  expenses = signal<Expense[]>([]);
  loading = signal(false);
  errorMessage = signal('');
  showDetailModal = signal(false);
  selectedExpense = signal<Expense | null>(null);
  editingExpense = false;

  // Filter and sort signals
  filters = signal<ExpenseFilter>({
    description: '',
    fromDate: '',
    toDate: '',
    minAmount: null,
    maxAmount: null,
    expenseCategoryId: '',
    status: ''
  });

  sortConfig = signal<SortConfig>({
    field: 'date',
    direction: 'desc'
  });

  pagination = signal({
    currentPage: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  });

  // Computed values
  sortedExpenses = computed(() => {
    const expenses = this.expenses();
    const sortConfig = this.sortConfig();

    if (!expenses.length) return [];

    return [...expenses].sort((a, b) => {
      let aValue: any, bValue: any;

      switch (sortConfig.field) {
        case 'description':
          aValue = a.description?.toLowerCase() || '';
          bValue = b.description?.toLowerCase() || '';
          break;
        case 'amount':
          aValue = a.amount;
          bValue = b.amount;
          break;
        case 'category':
          aValue = a.expenseCategory?.name?.toLowerCase() || '';
          bValue = b.expenseCategory?.name?.toLowerCase() || '';
          break;
        case 'date':
          aValue = new Date(a.date).getTime();
          bValue = new Date(b.date).getTime();
          break;
        case 'status':
          aValue = a.status?.toLowerCase() || '';
          bValue = b.status?.toLowerCase() || '';
          break;
        case 'user':
          aValue = a.user?.firstName?.toLowerCase() || '';
          bValue = b.user?.firstName?.toLowerCase() || '';
          break;
        default:
          return 0;
      }

      if (aValue < bValue) return sortConfig.direction === 'asc' ? -1 : 1;
      if (aValue > bValue) return sortConfig.direction === 'asc' ? 1 : -1;
      return 0;
    });
  });

  // Status options for filter
  statusOptions = [
    { value: '', label: 'All Statuses' },
    { value: 'Pending', label: 'Pending' },
    { value: 'Approved', label: 'Approved' },
    { value: 'Rejected', label: 'Rejected' }
  ];

  ngOnInit() {
    this.loadExpenses();
  }

  ngOnDestroy(): void {
    if (this.debounceTimer) {
      clearTimeout(this.debounceTimer);
    }
  }

   openExpenseDetail(expense: Expense) {
    this.selectedExpense.set(expense);
    this.showDetailModal.set(true);
  }

  closeExpenseDetail() {
    this.showDetailModal.set(false);
    this.selectedExpense.set(null);
    this.editingExpense = false;
  }

  openEditExpense(expense: Expense) {
    this.selectedExpense.set(expense);
    this.showDetailModal.set(true);
    this.editingExpense = true;
  }

  onExpenseUpdated(updatedExpense: Expense) {
    this.editingExpense = false;
    // Update the expense in the local list
    this.expenses.update(expenses =>
      expenses.map(exp => exp.id === updatedExpense.id ? updatedExpense : exp)
    );

    // Optionally refresh the list
    this.loadExpenses();
  }

  async loadExpenses() {
    this.loading.set(true);
    this.errorMessage.set('');

    try {
      const queryParams = this.buildQueryParams();
      const response: ExpenseCollection = await lastValueFrom(this.expenseService.getExpenses(queryParams));

      this.expenses.set(response.expenses || []);
      this.pagination.update(pagination => ({
        ...pagination,
        totalCount: response.totalCount,
        totalPages: response.totalPages
      }));
    } catch (error: any) {
      this.errorMessage.set(error.message || 'Failed to load expenses');
    } finally {
      this.loading.set(false);
    }
  }

  updateFilter(field: keyof ExpenseFilter, event: any): void {
    const value = event.target.value;

    this.filters.update(currentFilters => {
      const updatedFilters = { ...currentFilters };

      switch (field) {
        case 'minAmount':
        case 'maxAmount':
          updatedFilters[field] = value === '' ? null : +value;
          break;
        default:
          updatedFilters[field] = value;
          break;
      }

      return updatedFilters;
    });

    // Trigger filter change after a short debounce for better UX
    this.debouncedFilterChange();
  }


  buildQueryParams(): ExpenseQueryParams {
    const filters = this.filters();
    const pagination = this.pagination();

    return {
      Description: filters.description || undefined,
      FromDate: filters.fromDate || undefined,
      ToDate: filters.toDate || undefined,
      MinAmount: filters.minAmount || undefined,
      MaxAmount: filters.maxAmount || undefined,
      ExpenseCategoryId: filters.expenseCategoryId || undefined,
      Status: filters.status || undefined,
      Page: pagination.currentPage,
      PageSize: pagination.pageSize,
      SortBy: this.sortConfig().field,
      IsDescending: this.sortConfig().direction === 'desc'
    };
  }

  // Filter methods
  onFilterChange(): void {
    this.pagination.update(p => ({ ...p, currentPage: 1 }));
    this.loadExpenses();
  }

  clearFilters() {
    this.filters.set({
      description: '',
      fromDate: '',
      toDate: '',
      minAmount: null,
      maxAmount: null,
      expenseCategoryId: '',
      status: ''
    });
    this.onFilterChange();
  }

  // Sort methods
  onSort(field: string) {
    const currentSort = this.sortConfig();

    if (currentSort.field === field) {
      // Toggle direction if same field
      this.sortConfig.set({
        field,
        direction: currentSort.direction === 'asc' ? 'desc' : 'asc'
      });
    } else {
      // New field, default to ascending
      this.sortConfig.set({
        field,
        direction: 'asc'
      });
    }

    this.loadExpenses();
  }

  getSortIcon(field: string): string {
    const sortConfig = this.sortConfig();
    if (sortConfig.field !== field) return '↕️';
    return sortConfig.direction === 'asc' ? '↑' : '↓';
  }

  // Pagination methods
  onPageChange(page: number) {
    this.pagination.update(p => ({ ...p, currentPage: page }));
    this.loadExpenses();
  }

  onPageSizeChange(size: number) {
    this.pagination.update(p => ({ ...p, pageSize: size, currentPage: 1 }));
    this.loadExpenses();
  }

  // Utility methods
  formatCurrency(amount: number, currency: string): string {
    return new Intl.NumberFormat('fr-FR', {
      style: 'currency',
      currency: currency
    }).format(amount);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
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
}
