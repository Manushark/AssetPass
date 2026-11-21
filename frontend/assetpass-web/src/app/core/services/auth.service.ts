import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import {
  ApiResponse,
  LoginRequest,
  LoginResponse,
  RegisterTenantRequest,
  ResetPasswordRequest,
  AcceptInvitationRequest,
  User,
} from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = '/api';

  readonly currentUser = signal<User | null>(this.getStoredUser());
  readonly token = signal<string | null>(typeof localStorage !== 'undefined' ? localStorage.getItem('access_token') : null);
  readonly isAuthenticated = computed(() => !!this.currentUser());

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap((res) => {
          if (res.success && res.data) {
            this.handleAuthSuccess(res.data);
          }
        })
      );
  }

  registerTenant(data: RegisterTenantRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/auth/register`, data);
  }

  refreshToken(): Observable<ApiResponse<LoginResponse>> {
    const refreshToken = typeof localStorage !== 'undefined' ? localStorage.getItem('refresh_token') : null;
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.apiUrl}/auth/refresh-token`, { refreshToken })
      .pipe(
        tap((res) => {
          if (res.success && res.data) {
            this.handleAuthSuccess(res.data);
          } else {
            this.logout();
          }
        })
      );
  }

  logout(revokeAll = false): void {
    const refreshToken = typeof localStorage !== 'undefined' ? localStorage.getItem('refresh_token') : null;
    if (refreshToken) {
      this.http
        .post(`${this.apiUrl}/auth/logout`, { refreshToken, revokeAllSessions: revokeAll })
        .subscribe({ error: () => {} });
    }
    this.clearSession();
    this.router.navigate(['/auth/login']);
  }

  forgotPassword(email: string): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/auth/forgot-password`, { email });
  }

  resetPassword(req: ResetPasswordRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/auth/reset-password`, req);
  }

  acceptInvitation(req: AcceptInvitationRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/auth/accept-invitation`, req);
  }

  private handleAuthSuccess(data: LoginResponse): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.setItem('access_token', data.accessToken);
      localStorage.setItem('refresh_token', data.refreshToken);
      const user: User = {
        id: data.userId,
        email: data.email,
        role: data.role,
        tenantId: data.tenantId,
      };
      localStorage.setItem('user', JSON.stringify(user));
      this.token.set(data.accessToken);
      this.currentUser.set(user);
    }
  }

  private clearSession(): void {
    if (typeof localStorage !== 'undefined') {
      localStorage.removeItem('access_token');
      localStorage.removeItem('refresh_token');
      localStorage.removeItem('user');
    }
    this.token.set(null);
    this.currentUser.set(null);
  }

  private getStoredUser(): User | null {
    if (typeof localStorage === 'undefined') return null;
    const userStr = localStorage.getItem('user');
    if (!userStr) return null;
    try {
      return JSON.parse(userStr) as User;
    } catch {
      return null;
    }
  }
}
