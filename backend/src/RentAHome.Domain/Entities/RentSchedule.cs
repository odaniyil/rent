using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class RentSchedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantLeaseId { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? PaidOn { get; set; }
    public RentScheduleStatus Status { get; set; } = RentScheduleStatus.Pending;

    public TenantLease? TenantLease { get; set; }
}
