using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Warranty
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssetId { get; set; }
    public required string ProviderName { get; set; }
    public string? PolicyNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public WarrantyStatus Status { get; set; } = WarrantyStatus.Active;

    public Asset? Asset { get; set; }
}
