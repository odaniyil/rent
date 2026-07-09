using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Owner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public OwnerStatus Status { get; set; } = OwnerStatus.Active;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<OwnerLease> OwnerLeases { get; set; } = new List<OwnerLease>();
}
