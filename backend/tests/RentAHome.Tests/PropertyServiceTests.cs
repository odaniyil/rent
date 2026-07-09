using Microsoft.EntityFrameworkCore;
using RentAHome.Application.Properties;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;
using RentAHome.Persistence;
using RentAHome.Persistence.Properties;

namespace RentAHome.Tests;

public class PropertyServiceTests
{
    [Fact]
    public async Task CreateAsync_Creates_Property()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var service = new PropertyService(dbContext);

        var response = await service.CreateAsync(new CreatePropertyRequest(
            "New Apartment",
            "12 Main Street",
            null,
            "Central",
            "Austin",
            "TX",
            "78701",
            owner.Id,
            PropertyStatus.Available));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("New Apartment", response.Name);
        Assert.Equal("Central", response.Locality);
    }

    [Fact]
    public async Task UpdateAsync_Updates_Property()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id, "Original", "Austin", "Central", PropertyStatus.Available);
        var service = new PropertyService(dbContext);

        var response = await service.UpdateAsync(property.Id, new UpdatePropertyRequest(
            "Updated",
            "99 Updated Street",
            "Unit 2",
            "North",
            "Dallas",
            "TX",
            "75201",
            owner.Id,
            PropertyStatus.Maintenance));

        Assert.NotNull(response);
        Assert.Equal("Updated", response.Name);
        Assert.Equal("Dallas", response.City);
        Assert.Equal(PropertyStatus.Maintenance, response.Status);
    }

    [Fact]
    public async Task ListAsync_Filters_By_City_Locality_Status_Occupancy_And_LeaseExpiry()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var matched = await AddPropertyAsync(dbContext, owner.Id, "Matched", "Austin", "Central", PropertyStatus.Leased);
        await AddPropertyAsync(dbContext, owner.Id, "Other", "Dallas", "North", PropertyStatus.Available);
        dbContext.TenantLeases.Add(new TenantLease
        {
            PropertyId = matched.Id,
            TenantId = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 6, 30),
            MonthlyRent = 3000m,
            SecurityDeposit = 3000m,
            Status = TenantLeaseStatus.Active
        });
        await dbContext.SaveChangesAsync();
        var service = new PropertyService(dbContext);

        var response = await service.ListAsync(new PropertyListRequest(
            "Austin",
            "Central",
            PropertyStatus.Leased,
            "Occupied",
            new DateOnly(2026, 12, 31)));

        var item = Assert.Single(response);
        Assert.Equal(matched.Id, item.Id);
        Assert.Equal("Occupied", item.Occupancy);
    }

    [Fact]
    public async Task GetDetailAsync_Returns_Lease_Count_And_Profitability_Placeholders()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id, "Detail", "Austin", "Central", PropertyStatus.Leased);
        dbContext.OwnerLeases.Add(new OwnerLease
        {
            OwnerId = owner.Id,
            PropertyId = property.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            MonthlyRent = 2000m,
            SecurityDeposit = 2000m,
            Status = OwnerLeaseStatus.Active
        });
        dbContext.TenantLeases.Add(new TenantLease
        {
            TenantId = Guid.NewGuid(),
            PropertyId = property.Id,
            StartDate = new DateOnly(2026, 2, 1),
            EndDate = new DateOnly(2027, 1, 31),
            MonthlyRent = 2800m,
            SecurityDeposit = 2800m,
            Status = TenantLeaseStatus.Active
        });
        dbContext.Assets.Add(new Asset
        {
            PropertyId = property.Id,
            Name = "Sofa",
            Category = "Furniture",
            Status = AssetStatus.Installed
        });
        dbContext.MaintenanceTickets.Add(new MaintenanceTicket
        {
            PropertyId = property.Id,
            Title = "Fix sink",
            Status = MaintenanceTicketStatus.Open
        });
        await dbContext.SaveChangesAsync();
        var service = new PropertyService(dbContext);

        var response = await service.GetDetailAsync(property.Id);

        Assert.NotNull(response);
        Assert.NotNull(response.OwnerLease);
        Assert.NotNull(response.TenantLease);
        Assert.Equal(1, response.InventoryCount);
        Assert.Equal(1, response.MaintenanceCount);
        Assert.Equal(800m, response.ProfitabilitySummary.EstimatedMonthlyGrossProfit);
    }

    [Fact]
    public async Task SoftDeleteAsync_Excludes_Property_From_List()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddOwnerAsync(dbContext);
        var property = await AddPropertyAsync(dbContext, owner.Id, "Delete Me", "Austin", "Central", PropertyStatus.Available);
        var service = new PropertyService(dbContext);

        var deleted = await service.SoftDeleteAsync(property.Id);
        var properties = await service.ListAsync(new PropertyListRequest(null, null, null, null, null));

        Assert.True(deleted);
        Assert.Empty(properties);
        Assert.Equal(PropertyStatus.Inactive, await dbContext.Properties
            .Where(candidate => candidate.Id == property.Id)
            .Select(candidate => candidate.Status)
            .SingleAsync());
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

    private static async Task<Property> AddPropertyAsync(
        RentAHomeDbContext dbContext,
        Guid ownerId,
        string name,
        string city,
        string locality,
        PropertyStatus status)
    {
        var property = new Property
        {
            Name = name,
            AddressLine1 = "10 Test Street",
            Locality = locality,
            City = city,
            State = "TX",
            PostalCode = "78701",
            OwnerId = ownerId,
            Status = status
        };

        dbContext.Properties.Add(property);
        await dbContext.SaveChangesAsync();
        return property;
    }
}
