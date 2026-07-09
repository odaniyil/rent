using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class MaintenanceTicket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? VendorId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public MaintenanceTicketStatus Status { get; set; } = MaintenanceTicketStatus.Open;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Property? Property { get; set; }
    public Tenant? Tenant { get; set; }
    public Asset? Asset { get; set; }
    public Vendor? Vendor { get; set; }
}
