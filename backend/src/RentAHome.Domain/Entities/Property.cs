using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Locality { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAtUtc { get; set; }

    public Owner? Owner { get; set; }
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public ICollection<OwnerLease> OwnerLeases { get; set; } = new List<OwnerLease>();
    public ICollection<TenantLease> TenantLeases { get; set; } = new List<TenantLease>();
    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();
    public ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
    public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
}
