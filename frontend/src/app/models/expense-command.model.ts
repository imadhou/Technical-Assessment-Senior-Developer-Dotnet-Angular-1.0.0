export interface ExpenseCommand {
  description?: string | null;
  amount: number;
  expenseCategoryId: string;
  date: string; // ISO date string
  receiptUrl?: string | null;
  userId: string;
}
