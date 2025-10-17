import { ProblemDetails } from "./problem-details.model";
import { ValidationProblemDetails } from "./validation-problem-details.model";

// Type utilities
export type Optional<T, K extends keyof T> = Omit<T, K> & Partial<Pick<T, K>>;

export type RequiredBy<T, K extends keyof T> = T & Required<Pick<T, K>>;

export type Nullable<T> = { [P in keyof T]: T[P] | null };

export type DeepPartial<T> = {
  [P in keyof T]?: T[P] extends object ? DeepPartial<T[P]> : T[P];
};

export type ValueOf<T> = T[keyof T];

// Common enums based on your API
export enum ExpenseStatusEnum {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected'
}

export enum SortByOptionsEnum {
  Date = 'Date',
  Amount = 'Amount',
  Description = 'Description',
  Category = 'Category',
  CreatedAt = 'CreatedAt'
}

export enum UserRolesEnum {
  Admin = 'Admin',
  Manager = 'Manager',
  User = 'User'
}

// Date utilities for API compatibility
export function toISODateString(date: Date): string {
  return date.toISOString();
}

export function fromISODateString(isoString: string): Date {
  return new Date(isoString);
}

// Money formatting utilities
export function formatCurrency(amount: number, currency: string = 'USD'): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: currency
  }).format(amount);
}

// Status formatting utilities
export function getStatusColor(status: ExpenseStatusEnum | string): string {
  switch (status) {
    case ExpenseStatusEnum.Approved:
      return 'success';
    case ExpenseStatusEnum.Rejected:
      return 'danger';
    case ExpenseStatusEnum.Pending:
      return 'warning';
    default:
      return 'secondary';
  }
}

export function getStatusDisplayName(status: ExpenseStatusEnum | string): string {
  switch (status) {
    case ExpenseStatusEnum.Approved:
      return 'Approved';
    case ExpenseStatusEnum.Rejected:
      return 'Rejected';
    case ExpenseStatusEnum.Pending:
      return 'Pending Review';
    default:
      return status;
  }
}

// File size formatting
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes';

  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// API error handling
export interface ApiError {
  message: string;
  status: number;
  details?: ProblemDetails | ValidationProblemDetails;
}

export function createApiError(error: any): ApiError {
  if (error.error) {
    return {
      message: error.error.title || 'An error occurred',
      status: error.error.status || error.status,
      details: error.error
    };
  }

  return {
    message: error.message || 'An unexpected error occurred',
    status: error.status || 0
  };
}
