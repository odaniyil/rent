using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class TenantLease
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid PropertyId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal SecurityDeposit { get; set; }
    public bool SecurityDepositReceived { get; set; }
    public TenantLeaseStatus Status { get; set; } = TenantLeaseStatus.Draft;

    public Tenant? Tenant { get; set; }
    public Property? Property { get; set; }
    public ICollection<RentSchedule> RentSchedules { get; set; } = new List<RentSchedule>();
}
