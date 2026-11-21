import { describe, it, expect, beforeEach, vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from './auth.service';
import { LoginResponse } from '../models/auth.models';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'auth/login', component: class {} }]),
        AuthService,
      ],
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  it('should be created and start unauthenticated', () => {
    expect(service).toBeTruthy();
    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('should login and store tokens and user in localStorage and signals', () => {
    const mockResponse: { success: boolean; data: LoginResponse } = {
      success: true,
      data: {
        accessToken: 'access-123',
        refreshToken: 'refresh-456',
        expiresIn: 3600,
        userId: 'user-001',
        email: 'admin@assetpass.com',
        role: 'Owner',
        tenantId: 'tenant-999',
      },
    };

    service.login({ email: 'admin@assetpass.com', password: 'Password123!' }).subscribe((res) => {
      expect(res.success).toBe(true);
      expect(service.isAuthenticated()).toBe(true);
      expect(service.currentUser()?.email).toBe('admin@assetpass.com');
      expect(service.currentUser()?.role).toBe('Owner');
      expect(localStorage.getItem('access_token')).toBe('access-123');
      expect(localStorage.getItem('refresh_token')).toBe('refresh-456');
    });

    const req = httpTesting.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should clear session on logout', () => {
    localStorage.setItem('access_token', 'tok');
    localStorage.setItem('refresh_token', 'ref');
    localStorage.setItem('user', JSON.stringify({ id: '1', email: 'a@b.com', role: 'Owner', tenantId: 't1' }));

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
    expect(service.token()).toBeNull();
    expect(localStorage.getItem('access_token')).toBeNull();
  });
});
