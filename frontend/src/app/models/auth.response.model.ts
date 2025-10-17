export interface AuthResponse {
  token?: string | null;
  expires: string; // ISO date string
  userId?: string | null;
  email?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  role?: string | null;
}
