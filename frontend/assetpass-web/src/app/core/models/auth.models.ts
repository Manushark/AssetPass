export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: string;
  validationErrors?: string[];
}

export interface User {
  id: string;
  email: string;
  role: string;
  tenantId: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  userId: string;
  email: string;
  role: string;
  tenantId: string;
}

export interface RegisterTenantRequest {
  commercialName: string;
  legalName?: string;
  rnc?: string;
  phone: string;
  companyEmail: string;
  address?: string;
  currency: string;
  timeZone: string;
  adminFirstName: string;
  adminLastName: string;
  adminEmail: string;
  adminPassword: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface AcceptInvitationRequest {
  token: string;
  password: string;
  confirmPassword: string;
  firstName?: string;
  lastName?: string;
}
