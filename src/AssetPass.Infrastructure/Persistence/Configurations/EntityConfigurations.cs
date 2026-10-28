using AssetPass.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetPass.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.CommercialName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.LegalName).HasMaxLength(200);
        builder.Property(t => t.Rnc).HasMaxLength(50);
        builder.Property(t => t.Phone).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Email).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.LogoPath).HasMaxLength(500);
        builder.Property(t => t.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("DOP");
        builder.Property(t => t.TimeZone).IsRequired().HasMaxLength(100).HasDefaultValue("America/Santo_Domingo");
        builder.Property(t => t.Locale).IsRequired().HasMaxLength(20).HasDefaultValue("es-DO");
        builder.Property(t => t.SubscriptionStatus).HasConversion<int>();

        builder.HasOne(t => t.Plan)
            .WithMany(p => p.Tenants)
            .HasForeignKey(t => t.PlanId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.Email).IsUnique();
    }
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Phone).HasMaxLength(50);
        builder.Property(u => u.ProfilePhotoPath).HasMaxLength(500);
        builder.Property(u => u.IdentityUserId).IsRequired().HasMaxLength(450);
        builder.Property(u => u.Role).HasConversion<int>();

        builder.HasOne(u => u.Tenant)
            .WithMany(t => t.Users)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
        builder.HasIndex(u => u.IdentityUserId).IsUnique();
        builder.HasIndex(u => u.TenantId);
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Address).HasMaxLength(500);
        builder.Property(c => c.Notes).HasMaxLength(2000);
        builder.Property(c => c.ClientType).HasConversion<int>();

        builder.HasOne(c => c.Tenant)
            .WithMany(t => t.Clients)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => new { c.TenantId, c.Name });
    }
}

public class EstablishmentConfiguration : IEntityTypeConfiguration<Establishment>
{
    public void Configure(EntityTypeBuilder<Establishment> builder)
    {
        builder.ToTable("Establishments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.ContactPerson).HasMaxLength(200);
        builder.Property(e => e.Notes).HasMaxLength(2000);

        builder.HasOne(e => e.Client)
            .WithMany(c => c.Establishments)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.ClientId);
    }
}

public class EquipmentCategoryConfiguration : IEntityTypeConfiguration<EquipmentCategory>
{
    public void Configure(EntityTypeBuilder<EquipmentCategory> builder)
    {
        builder.ToTable("EquipmentCategories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.Icon).HasMaxLength(50);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => new { c.TenantId, c.Name }).IsUnique();
    }
}

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipment");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PublicId).IsRequired().HasMaxLength(30);
        builder.Property(e => e.Brand).HasMaxLength(100);
        builder.Property(e => e.Model).HasMaxLength(100);
        builder.Property(e => e.SerialNumber).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.PhotoPath).HasMaxLength(500);
        builder.Property(e => e.Status).HasConversion<int>();

        builder.HasOne(e => e.Client)
            .WithMany(c => c.Equipment)
            .HasForeignKey(e => e.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Establishment)
            .WithMany(est => est.Equipment)
            .HasForeignKey(e => e.EstablishmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Equipment)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Critical: PublicId must be globally unique (not just per tenant)
        builder.HasIndex(e => e.PublicId).IsUnique();
        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.ClientId);
        builder.HasIndex(e => new { e.TenantId, e.Status });
        builder.HasIndex(e => new { e.TenantId, e.SerialNumber });
    }
}

public class ServiceRecordConfiguration : IEntityTypeConfiguration<ServiceRecord>
{
    public void Configure(EntityTypeBuilder<ServiceRecord> builder)
    {
        builder.ToTable("ServiceRecords");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Description).HasMaxLength(4000);
        builder.Property(r => r.Observations).HasMaxLength(4000);
        builder.Property(r => r.PrivateNotes).HasMaxLength(4000);
        builder.Property(r => r.ServiceType).HasConversion<int>();

        builder.HasOne(r => r.Equipment)
            .WithMany(e => e.ServiceHistory)
            .HasForeignKey(r => r.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Technician)
            .WithMany()
            .HasForeignKey(r => r.TechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.ServiceOrder)
            .WithMany(o => o.ServiceRecords)
            .HasForeignKey(r => r.ServiceOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => r.EquipmentId);
        builder.HasIndex(r => new { r.EquipmentId, r.ServiceDate });
    }
}

public class ServiceRecordAttachmentConfiguration : IEntityTypeConfiguration<ServiceRecordAttachment>
{
    public void Configure(EntityTypeBuilder<ServiceRecordAttachment> builder)
    {
        builder.ToTable("ServiceRecordAttachments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
        builder.Property(a => a.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(a => a.ContentType).IsRequired().HasMaxLength(100);

        builder.HasOne(a => a.ServiceRecord)
            .WithMany(r => r.Attachments)
            .HasForeignKey(a => a.ServiceRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.TenantId);
    }
}

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.ToTable("ServiceOrders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Title).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Description).HasMaxLength(4000);
        builder.Property(o => o.Observations).HasMaxLength(4000);
        builder.Property(o => o.MaterialsUsed).HasMaxLength(2000);
        builder.Property(o => o.Result).HasMaxLength(4000);
        builder.Property(o => o.IdempotencyKey).HasMaxLength(100);
        builder.Property(o => o.ServiceType).HasConversion<int>();
        builder.Property(o => o.Status).HasConversion<int>();

        builder.HasOne(o => o.Equipment)
            .WithMany(e => e.ServiceOrders)
            .HasForeignKey(o => o.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Client)
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Establishment)
            .WithMany()
            .HasForeignKey(o => o.EstablishmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.AssignedTechnician)
            .WithMany()
            .HasForeignKey(o => o.AssignedTechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(o => o.TenantId);
        builder.HasIndex(o => new { o.TenantId, o.OrderNumber }).IsUnique();
        builder.HasIndex(o => new { o.TenantId, o.Status });
        builder.HasIndex(o => o.IdempotencyKey).IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");
    }
}

public class ServiceOrderAttachmentConfiguration : IEntityTypeConfiguration<ServiceOrderAttachment>
{
    public void Configure(EntityTypeBuilder<ServiceOrderAttachment> builder)
    {
        builder.ToTable("ServiceOrderAttachments");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName).IsRequired().HasMaxLength(255);
        builder.Property(a => a.StoragePath).IsRequired().HasMaxLength(500);
        builder.Property(a => a.ContentType).IsRequired().HasMaxLength(100);

        builder.HasOne(a => a.ServiceOrder)
            .WithMany(o => o.Attachments)
            .HasForeignKey(a => a.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.TenantId);
    }
}

public class WarrantyConfiguration : IEntityTypeConfiguration<Warranty>
{
    public void Configure(EntityTypeBuilder<Warranty> builder)
    {
        builder.ToTable("Warranties");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Provider).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Conditions).HasMaxLength(4000);
        builder.Property(w => w.DocumentPath).HasMaxLength(500);
        builder.Property(w => w.Notes).HasMaxLength(2000);
        builder.Property(w => w.Status).HasConversion<int>();

        builder.HasOne(w => w.Equipment)
            .WithMany(e => e.Warranties)
            .HasForeignKey(w => w.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(w => w.TenantId);
        builder.HasIndex(w => w.EquipmentId);
        builder.HasIndex(w => new { w.TenantId, w.EndDate });
    }
}

public class MaintenanceScheduleConfiguration : IEntityTypeConfiguration<MaintenanceSchedule>
{
    public void Configure(EntityTypeBuilder<MaintenanceSchedule> builder)
    {
        builder.ToTable("MaintenanceSchedules");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Description).HasMaxLength(2000);

        builder.HasOne(m => m.Equipment)
            .WithMany(e => e.MaintenanceSchedules)
            .HasForeignKey(m => m.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.AssignedTechnician)
            .WithMany()
            .HasForeignKey(m => m.AssignedTechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(m => m.TenantId);
        builder.HasIndex(m => new { m.TenantId, m.NextMaintenanceDate });
        builder.HasIndex(m => m.EquipmentId);
    }
}

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.Currency).IsRequired().HasMaxLength(10).HasDefaultValue("DOP");
        builder.Property(p => p.MonthlyPriceDop).HasPrecision(18, 2);

        builder.HasIndex(p => p.Name).IsUnique();
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Message).IsRequired().HasMaxLength(2000);
        builder.Property(n => n.ReferenceType).HasMaxLength(100);
        builder.Property(n => n.DeduplicationKey).HasMaxLength(200);
        builder.Property(n => n.Type).HasConversion<int>();

        builder.HasOne(n => n.Recipient)
            .WithMany()
            .HasForeignKey(n => n.RecipientUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => n.TenantId);
        builder.HasIndex(n => new { n.RecipientUserId, n.IsRead });
        builder.HasIndex(n => n.DeduplicationKey).IsUnique()
            .HasFilter("\"DeduplicationKey\" IS NOT NULL");
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Action).IsRequired().HasMaxLength(50);
        builder.Property(a => a.UserId).HasMaxLength(450);
        builder.Property(a => a.UserEmail).HasMaxLength(200);
        builder.Property(a => a.IpAddress).HasMaxLength(50);

        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
        builder.HasIndex(a => a.Timestamp);
    }
}

public class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
{
    public void Configure(EntityTypeBuilder<UserInvitation> builder)
    {
        builder.ToTable("UserInvitations");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Email).IsRequired().HasMaxLength(200);
        builder.Property(i => i.FirstName).HasMaxLength(100);
        builder.Property(i => i.LastName).HasMaxLength(100);
        builder.Property(i => i.Token).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Role).HasConversion<int>();

        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.Token).IsUnique();
        builder.HasIndex(i => new { i.TenantId, i.Email });
    }
}
