using AssetPass.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetPass.Application.Interfaces;

/// <summary>
/// Abstraction over the database context.
/// Used by application services to access data without depending on EF Core directly.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<Client> Clients { get; }
    DbSet<Establishment> Establishments { get; }
    DbSet<Equipment> Equipment { get; }
    DbSet<EquipmentCategory> EquipmentCategories { get; }
    DbSet<ServiceRecord> ServiceRecords { get; }
    DbSet<ServiceRecordAttachment> ServiceRecordAttachments { get; }
    DbSet<ServiceOrder> ServiceOrders { get; }
    DbSet<ServiceOrderAttachment> ServiceOrderAttachments { get; }
    DbSet<Warranty> Warranties { get; }
    DbSet<MaintenanceSchedule> MaintenanceSchedules { get; }
    DbSet<Plan> Plans { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<AuditLog> AuditLogs { get; }
    DbSet<UserInvitation> UserInvitations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
