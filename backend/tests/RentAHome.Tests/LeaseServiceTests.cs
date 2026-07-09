using Microsoft.EntityFrameworkCore;
using RentAHome.Application.Leases;
using RentAHome.Application.Properties;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;
using RentAHome.Persistence;
using RentAHome.Persistence.Leases;
using RentAHome.Persistence.Properties;

namespace RentAHome.Tests;

public class LeaseServiceTests
{
    [Fact]
    public async Task OwnerLease_CRUD_Works()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id);
        var service = new LeaseService(dbContext);

        var created = await service.CreateOwnerLeaseAsync(new CreateOwnerLeaseRequest(
            owner.Id,
            property.Id,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            2000m,
            2000m,
            true,
            OwnerLeaseStatus.Signed));

        Assert.True(created.Succeeded);
        Assert.NotEqual(Guid.Empty, created.Value!.Id);

        var updated = await service.UpdateOwnerLeaseAsync(created.Value.Id, new UpdateOwnerLeaseRequest(
            owner.Id,
            property.Id,
            new DateOnly(2026, 1, 1),
            new DateOnly(2027, 1, 31),
            2100m,
            2100m,
            false,
            OwnerLeaseStatus.Active));

        Assert.True(updated.Succeeded);
        Assert.Equal(2100m, updated.Value!.MonthlyRent);
        Assert.False(updated.Value.SubleaseAllowed);

        var listed = await service.ListOwnerLeasesAsync(new OwnerLeaseListRequest(owner.Id, property.Id, OwnerLeaseStatus.Active));
        Assert.Single(listed);

        Assert.True(await service.DeleteOwnerLeaseAsync(created.Value.Id));
        Assert.Null(await service.GetOwnerLeaseAsync(created.Value.Id));
    }

    [Fact]
    public async Task TenantLease_CRUD_Works()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var tenant = await AddTenantAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id);
        await AddOwnerLeaseAsync(dbContext, owner.Id, property.Id, true);
        var service = new LeaseService(dbContext);

        var created = await service.CreateTenantLeaseAsync(new CreateTenantLeaseRequest(
            tenant.Id,
            property.Id,
            new DateOnly(2026, 2, 1),
            new DateOnly(2027, 1, 31),
            2800m,
            2800m,
            false,
            TenantLeaseStatus.Draft));

        Assert.True(created.Succeeded);

        var updated = await service.UpdateTenantLeaseAsync(created.Value!.Id, new UpdateTenantLeaseRequest(
            tenant.Id,
            property.Id,
            new DateOnly(2026, 2, 1),
            new DateOnly(2027, 1, 31),
            2900m,
            2900m,
            true,
            TenantLeaseStatus.Active));

        Assert.True(updated.Succeeded);
        Assert.Equal(TenantLeaseStatus.Active, updated.Value!.Status);
        Assert.True(updated.Value.SecurityDepositReceived);

        var listed = await service.ListTenantLeasesAsync(new TenantLeaseListRequest(tenant.Id, property.Id, TenantLeaseStatus.Active));
        Assert.Single(listed);

        Assert.True(await service.DeleteTenantLeaseAsync(created.Value.Id));
        Assert.Null(await service.GetTenantLeaseAsync(created.Value.Id));
    }

    [Fact]
    public async Task Property_Cannot_Move_To_SetupInProgress_Without_Signed_OwnerLease()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id);
        var service = new PropertyService(dbContext);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(
            property.Id,
            new UpdatePropertyRequest(
                property.Name,
                property.AddressLine1,
                property.AddressLine2,
                property.Locality,
                property.City,
                property.State,
                property.PostalCode,
                owner.Id,
                PropertyStatus.SetupInProgress)));

        Assert.Equal("Property cannot move to SetupInProgress without a signed owner lease.", exception.Message);
    }

    [Fact]
    public async Task TenantLease_Cannot_Be_Created_When_Sublease_Is_Not_Allowed()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var tenant = await AddTenantAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id);
        await AddOwnerLeaseAsync(dbContext, owner.Id, property.Id, false);
        var service = new LeaseService(dbContext);

        var response = await service.CreateTenantLeaseAsync(CreateActiveTenantLeaseRequest(tenant.Id, property.Id));

        Assert.False(response.Succeeded);
        Assert.Equal("Tenant lease cannot be created until a signed owner lease allows subleasing.", response.Error);
    }

    [Fact]
    public async Task TenantLease_Cannot_Be_Activated_Without_SecurityDeposit()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var tenant = await AddTenantAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id);
        await AddOwnerLeaseAsync(dbContext, owner.Id, property.Id, true);
        var service = new LeaseService(dbContext);

        var response = await service.CreateTenantLeaseAsync(new CreateTenantLeaseRequest(
            tenant.Id,
            property.Id,
            new DateOnly(2026, 2, 1),
            new DateOnly(2027, 1, 31),
            2800m,
            2800m,
            false,
            TenantLeaseStatus.Active));

        Assert.False(response.Succeeded);
        Assert.Equal("Tenant lease cannot be activated until security deposit is marked received.", response.Error);
    }

    [Fact]
    public async Task Property_Cannot_Have_Two_Active_TenantLeases()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var tenant = await AddTenantAsync(dbContext);
        var otherTenant = await AddTenantAsync(dbContext, "other@example.com");
        var property = await AddPropertyAsync(dbContext, owner.Id);
        await AddOwnerLeaseAsync(dbContext, owner.Id, property.Id, true);
        dbContext.TenantLeases.Add(new TenantLease
        {
            TenantId = tenant.Id,
            PropertyId = property.Id,
            StartDate = new DateOnly(2026, 2, 1),
            EndDate = new DateOnly(2027, 1, 31),
            MonthlyRent = 2800m,
            SecurityDeposit = 2800m,
            SecurityDepositReceived = true,
            Status = TenantLeaseStatus.Active
        });
        await dbContext.SaveChangesAsync();
        var service = new LeaseService(dbContext);

        var response = await service.CreateTenantLeaseAsync(CreateActiveTenantLeaseRequest(otherTenant.Id, property.Id));

        Assert.False(response.Succeeded);
        Assert.Equal("Property already has an active tenant lease.", response.Error);
    }

    private static RentAHomeDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RentAHomeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RentAHomeDbContext(options);
    }

    private static async Task<Owner> AddOwnerAsync(RentAHomeDbContext dbContext)
    {
        var owner = new Owner
        {
            FullName = "Test Owner",
            Email = "owner@example.com",
            Status = OwnerStatus.Active
        };

        dbContext.Owners.Add(owner);
        await dbContext.SaveChangesAsync();
        return owner;
    }

    private static async Task<Tenant> AddTenantAsync(
        RentAHomeDbContext dbContext,
        string email = "tenant@example.com")
    {
        var tenant = new Tenant
        {
            FullName = "Test Tenant",
            Email = email,
            Status = TenantStatus.Active
        };

        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync();
        return tenant;
    }

    private static async Task<Property> AddPropertyAsync(RentAHomeDbContext dbContext, Guid ownerId)
    {
        var property = new Property
        {
            Name = "Test Property",
            AddressLine1 = "10 Test Street",
            Locality = "Central",
            City = "Austin",
            State = "TX",
            PostalCode = "78701",
            OwnerId = ownerId,
            Status = PropertyStatus.Available
        };

        dbContext.Properties.Add(property);
        await dbContext.SaveChangesAsync();
        return property;
    }

    private static async Task<OwnerLease> AddOwnerLeaseAsync(
        RentAHomeDbContext dbContext,
        Guid ownerId,
        Guid propertyId,
        bool subleaseAllowed)
    {
        var lease = new OwnerLease
        {
            OwnerId = ownerId,
            PropertyId = propertyId,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            MonthlyRent = 2000m,
            SecurityDeposit = 2000m,
            SubleaseAllowed = subleaseAllowed,
            Status = OwnerLeaseStatus.Signed
        };

        dbContext.OwnerLeases.Add(lease);
        await dbContext.SaveChangesAsync();
        return lease;
    }

    private static CreateTenantLeaseRequest CreateActiveTenantLeaseRequest(Guid tenantId, Guid propertyId)
    {
        return new CreateTenantLeaseRequest(
            tenantId,
            propertyId,
            new DateOnly(2026, 2, 1),
            new DateOnly(2027, 1, 31),
            2800m,
            2800m,
            true,
            TenantLeaseStatus.Active);
    }
}
