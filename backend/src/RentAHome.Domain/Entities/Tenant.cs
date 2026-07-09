using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<TenantLease> TenantLeases { get; set; } = new List<TenantLease>();
    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();
}
