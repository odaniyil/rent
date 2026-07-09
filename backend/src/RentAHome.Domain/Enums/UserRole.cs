namespace RentAHome.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 1,
    Founder = 2,
    OperationsManager = 3,
    FinanceManager = 4,
    PropertyAcquisitionManager = 5,
    InventoryManager = 6,
    MaintenanceManager = 7,
    FieldExecutive = 8,
    CustomerSupport = 9,
    VendorUser = 10,
    TenantUser = 11,
    OwnerUser = 12
}

public enum UserAccountStatus
{
    Active = 1,
    Inactive = 2
}
