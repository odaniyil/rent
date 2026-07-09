using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Inspection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public string? Notes { get; set; }
    public InspectionStatus Status { get; set; } = InspectionStatus.Scheduled;

    public Property? Property { get; set; }
}
