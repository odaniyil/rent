using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? PropertyId { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid? TenantId { get; set; }
    public required string FileName { get; set; }
    public required string StoragePath { get; set; }
    public DocumentStatus Status { get; set; } = DocumentStatus.Active;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
