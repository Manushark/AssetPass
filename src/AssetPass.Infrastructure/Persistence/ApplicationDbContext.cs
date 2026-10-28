using AssetPass.Application.Interfaces;
using AssetPass.Domain.Entities;
using AssetPass.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Infrastructure.Persistence;

/// <summary>
/// Application database context combining EF Core with ASP.NET Core Identity.
/// Implements multi-tenant global query filters for data isolation.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<AppIdentityUser>, IApplicationDbContext
{
    private readonly ITenantContext? _tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext? tenantContext = null)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    // Domain entities
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Establishment> Establishments => Set<Establishment>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<EquipmentCategory> EquipmentCategories => Set<EquipmentCategory>();
    public DbSet<ServiceRecord> ServiceRecords => Set<ServiceRecord>();
    public DbSet<ServiceRecordAttachment> ServiceRecordAttachments => Set<ServiceRecordAttachment>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ServiceOrderAttachment> ServiceOrderAttachments => Set<ServiceOrderAttachment>();
    public DbSet<Warranty> Warranties => Set<Warranty>();
    public DbSet<MaintenanceSchedule> MaintenanceSchedules => Set<MaintenanceSchedule>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply global query filters for multi-tenant isolation
        ApplyTenantFilters(modelBuilder);
    }

    /// <summary>
    /// Applies global query filters to all TenantEntity-derived entities.
    /// This ensures queries automatically filter by the current tenant.
    /// IMPORTANT: This is a defense-in-depth measure, not the sole protection.
    /// </summary>
    private void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        if (_tenantContext == null) return;

        // Client
        modelBuilder.Entity<Client>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // Establishment
        modelBuilder.Entity<Establishment>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // Equipment
        modelBuilder.Entity<Equipment>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // EquipmentCategory
        modelBuilder.Entity<EquipmentCategory>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // ServiceRecord
        modelBuilder.Entity<ServiceRecord>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // ServiceRecordAttachment
        modelBuilder.Entity<ServiceRecordAttachment>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // ServiceOrder
        modelBuilder.Entity<ServiceOrder>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // ServiceOrderAttachment
        modelBuilder.Entity<ServiceOrderAttachment>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // Warranty
        modelBuilder.Entity<Warranty>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // MaintenanceSchedule
        modelBuilder.Entity<MaintenanceSchedule>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // Notification
        modelBuilder.Entity<Notification>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // AuditLog
        modelBuilder.Entity<AuditLog>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // UserInvitation
        modelBuilder.Entity<UserInvitation>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

        // ApplicationUser
        modelBuilder.Entity<ApplicationUser>()
            .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-set audit fields
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    if (_tenantContext?.IsAuthenticated == true)
                        entry.Entity.CreatedBy = _tenantContext.UserEmail;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    if (_tenantContext?.IsAuthenticated == true)
                        entry.Entity.UpdatedBy = _tenantContext.UserEmail;
                    break;
            }
        }

        // Auto-set TenantId for new tenant-scoped entities
        if (_tenantContext?.IsAuthenticated == true)
        {
            foreach (var entry in ChangeTracker.Entries<Domain.Common.TenantEntity>())
            {
                if (entry.State == EntityState.Added && entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = _tenantContext.TenantId;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
