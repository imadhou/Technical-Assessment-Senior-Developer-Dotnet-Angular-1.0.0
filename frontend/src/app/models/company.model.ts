export interface Company {
  id: string;
  name?: string | null;
  domain?: string | null;
  description?: string | null;
  createdAt: string; // ISO date string
  updatedAt?: string | null; // ISO date string
}
