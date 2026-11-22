# Changelog

All notable changes to the AssetPass platform will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0-beta.1] - 2026-11-25

### Added
- **Authentication & User Management**:
  - `POST /api/auth/register`: Multi-tenant company onboarding with initial Owner user.
  - `POST /api/auth/login`: JWT access token & cryptographically random refresh token issuance.
  - `POST /api/auth/refresh-token`: Single-use refresh token rotation mechanism.
  - `POST /api/auth/logout`: Single session or global sessions token revocation.
  - `POST /api/auth/forgot-password`: Password reset request with user enumeration protection.
  - `POST /api/auth/reset-password`: Token-based password reset with automatic session invalidation.
  - `POST /api/auth/change-password`: Authenticated password change with active token revocation.
- **Role System**:
  - `RoleSeeder`: Automatic initialization of `Owner`, `Administrator`, `Technician`, and `Viewer` roles.
  - `GET /api/users`: Tenant-scoped paginated user query with multi-field search.
  - `PATCH /api/users/{userId}/role`: Role change endpoint with self-modification and privilege escalation guards.
- **User Invitation System**:
  - `POST /api/users/invite`: Owner and Administrator can invite members to their company with specific roles (excluding Owner). Generates secure cryptographic tokens and sends invitation email.
  - `POST /api/auth/accept-invitation`: Unauthenticated endpoint to accept invitation tokens and set account passwords.
  - Command handlers with FluentValidation: `InviteUserCommandHandler` and `AcceptInvitationCommandHandler`.
- **Angular Auth Module**:
  - `AuthService`: Signals-based authentication service (`currentUser`, `token`, `isAuthenticated`) with token persistence and session lifecycle.
  - `jwtInterceptor`: Functional HTTP interceptor attaching JWT Bearer tokens and managing 401 unauthenticated redirects.
  - `authGuard`: Route guard protecting internal routes against unauthenticated access with returnUrl redirection.
  - Standalone UI components: `LoginComponent`, `RegisterComponent` (company onboarding), `ForgotPasswordComponent`, and protected `DashboardComponent`.
- **Testing**:
  - Unit test suite for password reset, forgot password, and change password flows.
  - Unit test suite for tenant user role change authorization guards.
  - Unit test suite for `InviteUserCommandHandler` and `AcceptInvitationCommandHandler`.
  - Unit test suite for `HttpTenantContext` claim extraction and isolation defaults.
  - Vitest test suite for Angular `AuthService`, `authGuard`, `jwtInterceptor`, and `LoginComponent` (10/10 tests passing).
  - Unit test suite for `InMemoryRefreshTokenStore` lifecycle and token cleanup.
  - Unit test suite for `ApiResponse` envelope contract.

