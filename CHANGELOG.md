# Changelog

All notable changes to the AssetPass platform will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
- **Testing**:
  - Unit test suite for password reset, forgot password, and change password flows.
  - Unit test suite for tenant user role change authorization guards.
  - Unit test suite for `InMemoryRefreshTokenStore` lifecycle and token cleanup.
  - Unit test suite for `ApiResponse` envelope contract.
