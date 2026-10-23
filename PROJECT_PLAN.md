# AssetPass — Project Plan

## Product Vision

AssetPass is a multi-tenant SaaS platform for managing the digital history of equipment, machines, and devices requiring installation, maintenance, repair, and warranty tracking. Each registered piece of equipment gets a unique **digital passport** accessible via QR code.

## Technology Stack

| Layer | Technology | Version |
|---|---|---|
| Backend Runtime | .NET | 9.0 |
| Backend Framework | ASP.NET Core Web API | 9.0 |
| ORM | Entity Framework Core | 9.0 |
| Authentication | ASP.NET Core Identity + JWT | 9.0 |
| Validation | FluentValidation | Latest stable |
| Frontend Framework | Angular | 21.x (LTS) |
| Language | TypeScript | Strict mode |
| Database | PostgreSQL | 16.x |
| Containerization | Docker + Docker Compose | Latest |
| Testing (Backend) | xUnit + FluentAssertions | Latest |
| Testing (E2E) | Playwright | Latest |
| API Documentation | Swagger / OpenAPI | Via Swashbuckle |
| QR Generation | QRCoder | Latest stable |
| PDF Generation | QuestPDF | Latest stable |

## Architecture

**Clean Architecture** with four main layers:

```
src/
├── AssetPass.Domain/          # Entities, Value Objects, Enums, Domain Rules
├── AssetPass.Application/     # Use Cases, DTOs, Interfaces, Validators
├── AssetPass.Infrastructure/  # EF Core, Repositories, External Services
└── AssetPass.Api/             # Controllers, Middleware, Configuration
tests/
├── AssetPass.Domain.Tests/
├── AssetPass.Application.Tests/
├── AssetPass.Infrastructure.Tests/
└── AssetPass.Api.Tests/
frontend/
└── assetpass-web/             # Angular 21 SPA
```

## Multitenancy Strategy

- **Shared database, discriminator column** (`TenantId`) on all tenant-scoped entities
- EF Core global query filters as an additional safety layer
- `TenantId` derived from authenticated user's JWT claims (never from client input)
- Database-level constraints and indexes on `TenantId`
- Explicit cross-tenant access tests

## Implementation Phases

### PHASE 0 — Research & Planning ✅
- [x] Inspect environment
- [x] Review available versions
- [x] Technology decisions
- [x] Create project plan
- [x] Define architecture
- [x] Identify risks
- [x] Create initial documentation

### PHASE 1 — Foundation
- [ ] Initialize Git repository
- [ ] Create .NET solution structure (Clean Architecture)
- [ ] Create Angular project
- [ ] Configure Docker & Docker Compose
- [ ] Set up PostgreSQL container
- [ ] Create Domain entities (core models)
- [ ] Create DbContext with tenant isolation
- [ ] Create initial EF Core migration
- [ ] Configure Swagger/OpenAPI
- [ ] Create .env.example and configuration system
- [ ] Verify database creation from scratch

### PHASE 2 — Authentication & Multi-tenancy
- [ ] ASP.NET Core Identity setup with PostgreSQL
- [ ] JWT authentication configuration
- [ ] Company (Tenant) registration endpoint
- [ ] User registration (admin user for new company)
- [ ] Login / Logout endpoints
- [ ] Password recovery flow
- [ ] Password change
- [ ] Role system (Owner, Administrator, Technician, Viewer)
- [ ] User management within a company
- [ ] User invitation system
- [ ] Tenant isolation middleware
- [ ] Angular auth module (login, register, guards, interceptors)
- [ ] Brute force protection
- [ ] Cross-tenant access tests

### PHASE 3 — Business Management
- [ ] Companies module (profile, settings)
- [ ] Clients CRUD + search/filter/pagination
- [ ] Establishments CRUD
- [ ] Equipment CRUD with categories
- [ ] Equipment categories (configurable)
- [ ] Equipment states management
- [ ] Photo upload for equipment
- [ ] Angular components for all entities
- [ ] Integration tests

### PHASE 4 — Digital Passport
- [ ] Public identifier generation (secure, non-sequential)
- [ ] QR code generation
- [ ] QR download as PNG
- [ ] Printable PDF generation with label
- [ ] QR revocation and regeneration
- [ ] Public equipment portal (mobile-first)
- [ ] Public data visibility configuration
- [ ] Data exposure protection
- [ ] Rate limiting on public endpoints
- [ ] Angular public portal page

### PHASE 5 — Operations
- [ ] Service orders CRUD
- [ ] Service order states workflow
- [ ] Service history (chronological log)
- [ ] Audit trail for modifications
- [ ] Preventive maintenance scheduling
- [ ] Maintenance frequency configuration
- [ ] Upcoming/overdue maintenance detection
- [ ] Warranty management
- [ ] Warranty expiration alerts
- [ ] Photo/document attachments
- [ ] Angular components for all operations

### PHASE 6 — Dashboard & Notifications
- [ ] Dashboard metrics API (tenant-scoped)
- [ ] Equipment statistics
- [ ] Maintenance status overview
- [ ] Service order statistics
- [ ] Warranty alerts
- [ ] Recent activity feed
- [ ] Notification system architecture
- [ ] Background job scheduler
- [ ] Duplicate notification prevention
- [ ] Angular dashboard with real data

### PHASE 7 — Subscriptions
- [ ] Plan entity and configuration
- [ ] Subscription management
- [ ] Feature limits per plan
- [ ] Plan enforcement (backend validation)
- [ ] Trial period support
- [ ] Subscription status tracking
- [ ] Prepared payment integration interface
- [ ] Angular subscription management

### PHASE 8 — QA & Security
- [ ] Comprehensive unit tests
- [ ] Integration test suite
- [ ] E2E tests with Playwright
- [ ] Cross-tenant security tests
- [ ] IDOR prevention verification
- [ ] Privilege escalation tests
- [ ] Public portal security audit
- [ ] File upload security
- [ ] Rate limiting verification
- [ ] CORS configuration review
- [ ] Error handling audit

### PHASE 9 — Commercial Readiness
- [ ] Landing page
- [ ] Onboarding flow
- [ ] Demo data seeder (dev only)
- [ ] Complete documentation
- [ ] Deployment guide
- [ ] Performance review
- [ ] External dependencies checklist
- [ ] Final security review

## Risk Register

| Risk | Impact | Mitigation |
|---|---|---|
| .NET 9 EOL Nov 2026 | Medium | Plan migration to .NET 10 LTS when available |
| No payment processor | Low | Internal subscription management; payment integration designed as pluggable |
| No email provider | Medium | Interface + mock; document required credentials |
| Multi-tenant data leak | Critical | Global query filters + middleware + tests + DB constraints |
| File storage scalability | Medium | Abstract storage interface; local for dev, cloud-ready |
| QR public endpoint abuse | Medium | Rate limiting + abuse detection |

## Definition of Done

A module is **Completed** when:
1. Backend API endpoints are implemented and working
2. Frontend components are connected to real API data
3. Database migrations are created and tested
4. Unit tests pass
5. Integration tests pass
6. Tenant isolation is verified
7. No known critical bugs
8. Documentation is updated
