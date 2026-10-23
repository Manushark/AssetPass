# AssetPass — Architecture Decision Records

## ADR-001: .NET 9 for Backend

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Both .NET 8 (LTS) and .NET 9 (STS) reach EOL on November 10, 2026. .NET 10 (next LTS) is not yet released.

**Decision:** Use .NET 9 as it provides the latest APIs and same support window as .NET 8.

**Consequences:** Plan migration to .NET 10 LTS when released. The upgrade path from 9 to 10 is straightforward.

---

## ADR-002: Angular 21 LTS for Frontend

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Angular 22 is the latest stable, but Angular 21 is in LTS phase with support until mid-2027.

**Decision:** Use Angular 21 for stability and longer support window.

**Consequences:** Angular 21 supports signals, standalone components, and all modern Angular features. Can upgrade to 22+ later.

---

## ADR-003: Shared Database Multi-tenancy with TenantId Column

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Three common multi-tenancy strategies:
1. **Separate databases per tenant** — Maximum isolation but highest cost/complexity
2. **Separate schemas per tenant** — Good isolation, moderate complexity
3. **Shared database with discriminator column** — Lowest cost, suitable for SMB SaaS

**Decision:** Use shared database with `TenantId` discriminator column on all tenant-scoped entities.

**Rationale:**
- Target market is small businesses (low data volume per tenant)
- Simplifies operations (single database to manage)
- Lower infrastructure cost
- EF Core global query filters provide an additional safety layer
- Cross-cutting queries (admin, reporting) are simpler

**Mitigations:**
- `TenantId` derived from JWT, never from client
- EF Core global query filters as defense-in-depth
- Database-level indexes including TenantId
- Explicit cross-tenant access tests
- Row-level policies can be added later if needed

---

## ADR-004: Clean Architecture without MediatR/CQRS

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** MediatR and CQRS add indirection. For an MVP with moderate complexity, they add more ceremony than value.

**Decision:** Use simple service classes in the Application layer. Introduce MediatR/CQRS only if complexity justifies it later.

**Consequences:** Simpler codebase, fewer files per feature, easier to understand for new developers.

---

## ADR-005: JWT Authentication for SPA

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Angular SPA needs stateless authentication with the API.

**Decision:** Use short-lived JWT access tokens with refresh tokens stored in HttpOnly cookies.

**Rationale:**
- Access token in memory (not localStorage) — XSS safe
- Refresh token in HttpOnly, Secure, SameSite cookie — CSRF-resistant
- Standard approach for SPAs with ASP.NET Core

---

## ADR-006: QR Code Public Identifiers

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Equipment QR codes need a public URL with an identifier that is:
- Non-sequential (prevents enumeration)
- Difficult to guess
- Not containing personal data
- Revocable

**Decision:** Use a combination of a URL-safe random string (NanoID, 21 chars) as the public identifier.

**Rationale:**
- NanoID with 21 characters provides ~126 bits of entropy
- URL-safe alphabet (A-Za-z0-9_-)
- Not sequential, not guessable
- Can be regenerated (revoke old, generate new)
- Compact enough for QR codes

---

## ADR-007: File Storage Abstraction

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Equipment photos, documents, and attachments need storage.

**Decision:** Define an `IFileStorageService` interface. Implement local file system storage for development. Design for cloud storage (S3, Azure Blob) later.

**Consequences:** Files stored in a configurable directory outside wwwroot. Served through authorized endpoints, never directly.

---

## ADR-008: Notification System Design

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Notifications needed for maintenance reminders, warranty alerts, and order updates.

**Decision:** Design a decoupled notification system with:
- `INotificationService` interface
- Notification entity tracking sent notifications (prevents duplicates)
- Background job for scheduled checks
- Channel-agnostic design (email, WhatsApp, in-app)

**Initial implementation:** In-app notifications only. Email and WhatsApp channels prepared as interfaces.

---

## ADR-009: No Generic Repository Pattern

**Status:** Accepted  
**Date:** 2026-10-30

**Context:** Generic repositories often wrap EF Core's DbContext without adding value, creating unnecessary abstraction.

**Decision:** Use DbContext directly in Application services via `IApplicationDbContext` interface. Create specific repository/service classes only when they encapsulate meaningful logic beyond CRUD.

**Consequences:** Less boilerplate, DbContext is already a Unit of Work + Repository pattern.
