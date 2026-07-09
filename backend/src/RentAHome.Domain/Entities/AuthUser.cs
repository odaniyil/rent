using RentAHome.Domain.Enums;

namespace RentAHome.Domain.Entities;

public sealed class AuthUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public UserAccountStatus Status { get; set; } = UserAccountStatus.Active;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
