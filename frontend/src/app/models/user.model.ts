import { Company } from "./company.model";
import { Role } from "./role.model";

export interface User {
  id: string;
  email?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  companyId: string;
  roleId: string;
  createdAt: string; // ISO date string
  updatedAt?: string | null; // ISO date string
  company?: Company;
  role?: Role;
}
