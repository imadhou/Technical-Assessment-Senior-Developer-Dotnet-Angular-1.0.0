export interface ExpenseCategory {
  id: string;
  name?: string | null;
  description?: string | null;
  companyId: string;
  createdAt: string; // ISO date string
}
