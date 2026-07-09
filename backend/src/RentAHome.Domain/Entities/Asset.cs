using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public Guid? RoomId { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public string? SerialNumber { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.Inventory;

    public Property? Property { get; set; }
    public Room? Room { get; set; }
    public ICollection<MaintenanceTicket> MaintenanceTickets { get; set; } = new List<MaintenanceTicket>();
    public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
}
