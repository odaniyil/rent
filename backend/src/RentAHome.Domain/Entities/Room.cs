using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PropertyId { get; set; }
    public required string Name { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Active;

    public Property? Property { get; set; }
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
