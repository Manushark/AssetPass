# Changelog

All notable changes to the AssetPass platform will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- **Company Management**:
  - `GET /api/company`: Returns authenticated tenant company profile and configuration.
  - `PUT /api/company`: Updates company commercial details, contact info, currency, timezone, and locale.
  - `PATCH /api/company/public-portal`: Configures public equipment portal information visibility flags.
- **Client Management**:
  - `GET /api/clients`: Returns tenant-scoped paginated clients with search and filtering by client type and status.
  - `GET /api/clients/{id}`: Returns client detail profile with linked establishments and equipment counts.
  - `POST /api/clients`: Creates new client with FluentValidation rules.
  - `PUT /api/clients/{id}`: Updates client contact details and active state.
  - `DELETE /api/clients/{id}`: Soft-deletes (deactivates) client while preserving asset traceability.
- **Establishment Management**:
  - `GET /api/establishments`: Returns paginated establishments with optional filtering by `clientId`, search term, and active status.
  - `GET /api/establishments/{id}`: Returns detailed establishment info with equipment count.
  - `POST /api/establishments`: Creates a new establishment verifying client ownership within tenant.
  - `PUT /api/establishments/{id}`: Updates establishment address, contact person, and status.
  - `DELETE /api/establishments/{id}`: Soft-deletes establishment preserving equipment history.
- **Testing**:
  - Unit tests for `UpdateCompanyProfileCommandValidator` data integrity.
  - Unit tests for `CreateClientCommandValidator` and `UpdateClientCommandValidator`.
  - Unit tests for `CreateEstablishmentCommandValidator` and `UpdateEstablishmentCommandValidator`.

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

