import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import {
  LoginCommand,
  RegisterCommand,
  AuthResponse,
  User,
  API_CONFIG
} from '../models/model.index';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly authEndpoint = API_CONFIG.ENDPOINTS.AUTH;

  constructor(private baseApi: BaseApiService) {}

  login(loginCommand: LoginCommand): Observable<AuthResponse> {
    return this.baseApi.post<AuthResponse>(
      this.authEndpoint.LOGIN,
      loginCommand
    ).pipe(
      tap(response => this.storeAuthData(response))
    );
  }

  register(registerCommand: RegisterCommand): Observable<AuthResponse> {
    return this.baseApi.post<AuthResponse>(
      this.authEndpoint.REGISTER,
      registerCommand
    ).pipe(
      tap(response => this.storeAuthData(response))
    );
  }

  getCurrentUser(): Observable<User> {
    return this.baseApi.get<User>(this.authEndpoint.ME);
  }

  logout(): void {
    this.clearAuthData();
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    // Check if token is expired (basic check)
    const expiry = this.getTokenExpiry();
    if (expiry && new Date() > new Date(expiry)) {
      this.clearAuthData();
      return false;
    }

    return true;
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  private storeAuthData(authResponse: AuthResponse): void {
    if (authResponse.token) {
      localStorage.setItem('auth_token', authResponse.token);
    }
    if (authResponse.expires) {
      localStorage.setItem('token_expiry', authResponse.expires);
    }
    if (authResponse.userId) {
      localStorage.setItem('user_id', authResponse.userId);
    }
  }

  private clearAuthData(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('token_expiry');
    localStorage.removeItem('user_id');
    localStorage.removeItem('user_data');
  }

  private getTokenExpiry(): string | null {
    return localStorage.getItem('token_expiry');
  }
}
