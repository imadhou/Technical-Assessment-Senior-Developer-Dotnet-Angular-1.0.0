import { Expense } from "./expense.model";
import { Link } from "./link.model";

export interface ExpenseCollection {
  expenses?: Expense[] | null;
  links?: Link[] | null;
  totalCount: number;
  totalPages: number;
}
