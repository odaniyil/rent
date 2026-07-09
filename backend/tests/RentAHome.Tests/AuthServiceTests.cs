using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RentAHome.Application.Auth;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;
using RentAHome.Persistence;
using RentAHome.Persistence.Auth;

namespace RentAHome.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_Returns_Jwt_For_Valid_Credentials()
    {
        await using var dbContext = CreateDbContext();
        dbContext.AuthUsers.Add(new AuthUser
        {
            Email = "admin@example.com",
            PasswordHash = PasswordHasher.Hash("CorrectHorse1!"),
            Role = UserRole.SuperAdmin,
            Status = UserAccountStatus.Active
        });
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var response = await service.LoginAsync(new LoginRequest("admin@example.com", "CorrectHorse1!"));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.Equal("admin@example.com", response.Email);
        Assert.Equal(nameof(UserRole.SuperAdmin), response.Role);
    }

    [Fact]
    public async Task LoginAsync_Returns_Null_For_Invalid_Password()
    {
        await using var dbContext = CreateDbContext();
        dbContext.AuthUsers.Add(new AuthUser
        {
            Email = "admin@example.com",
            PasswordHash = PasswordHasher.Hash("CorrectHorse1!"),
            Role = UserRole.SuperAdmin,
            Status = UserAccountStatus.Active
        });
        await dbContext.SaveChangesAsync();

        var service = CreateService(dbContext);
        var response = await service.LoginAsync(new LoginRequest("admin@example.com", "wrong-password"));

        Assert.Null(response);
    }

    private static RentAHomeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RentAHomeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RentAHomeDbContext(options);
    }

    private static AuthService CreateService(RentAHomeDbContext dbContext)
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "RentAHome.Tests",
            Audience = "RentAHome.Tests",
            SigningKey = "test-only-rent-a-home-jwt-signing-key",
            ExpirationMinutes = 15
        });

        return new AuthService(dbContext, options);
    }
}
