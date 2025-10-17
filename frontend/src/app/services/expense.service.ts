import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_CONFIG, Expense, ExpenseCollection, ExpenseCommand, ExpenseQueryParams, ExpenseStatusUpdateCommand } from '../models/model.index';
import { BaseApiService } from './base-api.service';


@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private readonly expenseEndpoint = API_CONFIG.ENDPOINTS.EXPENSES;

  constructor(private baseApi: BaseApiService) {}

  getExpenses(queryParams: ExpenseQueryParams): Observable<ExpenseCollection> {
    return this.baseApi.get<ExpenseCollection>(
      this.expenseEndpoint.BASE,
      queryParams
    );
  }

  getExpenseById(id: string): Observable<Expense> {
    return this.baseApi.get<Expense>(`${this.expenseEndpoint.BASE}/${id}`);
  }

  createExpense(expenseCommand: ExpenseCommand): Observable<Expense> {
    return this.baseApi.post<Expense>(
      this.expenseEndpoint.BASE,
      expenseCommand
    );
  }

  updateExpense(id: string, expenseCommand: ExpenseCommand): Observable<Expense> {
    return this.baseApi.put<Expense>(
      `${this.expenseEndpoint.BASE}/${id}`,
      expenseCommand
    );
  }

  updateExpenseStatus(id: string, statusUpdate: ExpenseStatusUpdateCommand): Observable<Expense> {
    const url = this.expenseEndpoint.STATUS.replace('{id}', id);
    return this.baseApi.patch<Expense>(url, statusUpdate);
  }

  deleteExpense(id: string): Observable<void> {
    return this.baseApi.delete<void>(`${this.expenseEndpoint.BASE}/${id}`);
  }

  // Helper method to build query params from filter object
  buildQueryParams(filters: any): ExpenseQueryParams {
    const params: ExpenseQueryParams = {};

    if (filters.description) params.Description = filters.description;
    if (filters.fromDate) params.FromDate = filters.fromDate;
    if (filters.toDate) params.ToDate = filters.toDate;
    if (filters.minAmount) params.MinAmount = filters.minAmount;
    if (filters.maxAmount) params.MaxAmount = filters.maxAmount;
    if (filters.expenseCategoryId) params.ExpenseCategoryId = filters.expenseCategoryId;
    if (filters.userId) params.UserId = filters.userId;
    if (filters.status) params.Status = filters.status;
    if (filters.page) params.Page = filters.page;
    if (filters.pageSize) params.PageSize = filters.pageSize;
    if (filters.sortBy) params.SortBy = filters.sortBy;
    if (filters.isDescending !== undefined) params.IsDescending = filters.isDescending;

    return params;
  }
}
