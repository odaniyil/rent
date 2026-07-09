namespace RentAHome.Domain.Enums;

public enum PropertyStatus
{
    Available = 1,
    Leased = 2,
    Maintenance = 3,
    Inactive = 4
}

public enum OwnerStatus
{
    Active = 1,
    Inactive = 2
}

public enum OwnerLeaseStatus
{
    Draft = 1,
    Active = 2,
    Expired = 3,
    Terminated = 4
}

public enum TenantStatus
{
    Active = 1,
    Inactive = 2
}

public enum TenantLeaseStatus
{
    Draft = 1,
    Active = 2,
    Expired = 3,
    Terminated = 4
}

public enum AssetStatus
{
    Inventory = 1,
    Installed = 2,
    Maintenance = 3,
    Retired = 4
}

public enum MaintenanceTicketStatus
{
    Open = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4,
    Cancelled = 5
}

public enum RentScheduleStatus
{
    Pending = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4
}

public enum DocumentStatus
{
    Active = 1,
    Archived = 2
}

public enum VendorStatus
{
    Active = 1,
    Inactive = 2
}

public enum WarrantyStatus
{
    Active = 1,
    Expired = 2,
    Claimed = 3
}

public enum InsurancePolicyStatus
{
    Active = 1,
    Expired = 2,
    Cancelled = 3
}

public enum RoomStatus
{
    Active = 1,
    Inactive = 2
}

public enum InspectionStatus
{
    Scheduled = 1,
    Passed = 2,
    Failed = 3,
    Completed = 4,
    Cancelled = 5
}

public enum AuditLogStatus
{
    Recorded = 1,
    Archived = 2
}
