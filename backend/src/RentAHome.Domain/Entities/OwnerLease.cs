using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class OwnerLease
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public Guid PropertyId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal SecurityDeposit { get; set; }
    public OwnerLeaseStatus Status { get; set; } = OwnerLeaseStatus.Draft;

    public Owner? Owner { get; set; }
    public Property? Property { get; set; }
}
