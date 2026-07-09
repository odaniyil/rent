using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Vendor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public VendorStatus Status { get; set; } = VendorStatus.Active;

    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();
}
