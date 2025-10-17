export interface ExpenseQueryParams {
  Description?: string;
  FromDate?: string; // ISO date string
  ToDate?: string; // ISO date string
  MinAmount?: number;
  MaxAmount?: number;
  ExpenseCategoryId?: string;
  UserId?: string;
  Status?: string;
  Page?: number;
  PageSize?: number;
  SortBy?: string;
  IsDescending?: boolean;
}
