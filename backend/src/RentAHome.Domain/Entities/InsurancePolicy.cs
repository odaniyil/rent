using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class InsurancePolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public required string ProviderName { get; set; }
    public required string PolicyNumber { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public InsurancePolicyStatus Status { get; set; } = InsurancePolicyStatus.Active;

    public Property? Property { get; set; }
}
