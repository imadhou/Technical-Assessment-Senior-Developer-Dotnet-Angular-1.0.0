import { ExpenseCategory } from "./expense-category.model";
import { Link } from "./link.model";
import { User } from "./user.model";

export interface Expense {
  id: string;
  description?: string | null;
  amount: number;
  currency: string;
  expenseCategoryId: string;
  userId: string;
  date: string; // ISO date string
  status?: string | null;
  receiptUrl?: string | null;
  receiptFileName?: string | null;
  receiptFileSize?: number | null;
  receiptUploadedAt?: string | null; // ISO date string
  createdAt: string; // ISO date string
  updatedAt?: string | null; // ISO date string
  expenseCategory: ExpenseCategory;
  user: User;
  links: Link[];
}
