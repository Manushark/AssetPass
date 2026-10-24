namespace AssetPass.Domain.Enums;

public enum NotificationType
{
    MaintenanceDue = 1,
    MaintenanceOverdue = 2,
    WarrantyExpiringSoon = 3,
    WarrantyExpired = 4,
    OrderAssigned = 5,
    OrderCompleted = 6,
    OrderCancelled = 7
}
