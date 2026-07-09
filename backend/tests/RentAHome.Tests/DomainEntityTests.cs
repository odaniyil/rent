using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;

namespace RentAHome.Tests;

public class DomainEntityTests
{
    [Fact]
    public void Property_Can_Be_Created_With_Owner()
    {
        var ownerId = Guid.NewGuid();

        var property = new Property
        {
            Name = "Test Apartment",
            AddressLine1 = "10 Test Street",
            City = "Austin",
            State = "TX",
            PostalCode = "78701",
            OwnerId = ownerId,
            Status = PropertyStatus.Available
        };

        Assert.NotEqual(Guid.Empty, property.Id);
        Assert.Equal(ownerId, property.OwnerId);
        Assert.Equal(PropertyStatus.Available, property.Status);
    }

    [Fact]
    public void TenantLease_Can_Be_Created_With_Rent_Schedule()
    {
        var lease = new TenantLease
        {
            TenantId = Guid.NewGuid(),
            PropertyId = Guid.NewGuid(),
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            MonthlyRent = 3200m,
            SecurityDeposit = 3200m,
            Status = TenantLeaseStatus.Active
        };

        lease.RentSchedules.Add(new RentSchedule
        {
            TenantLeaseId = lease.Id,
            DueDate = new DateOnly(2026, 2, 1),
            Amount = 3200m,
            Status = RentScheduleStatus.Pending
        });

        Assert.Single(lease.RentSchedules);
        Assert.Equal(TenantLeaseStatus.Active, lease.Status);
    }

    [Fact]
    public void MaintenanceTicket_Can_Be_Created_For_Asset()
    {
        var assetId = Guid.NewGuid();

        var ticket = new MaintenanceTicket
        {
            PropertyId = Guid.NewGuid(),
            AssetId = assetId,
            Title = "Repair sofa leg",
            Description = "One sofa leg is loose.",
            Status = MaintenanceTicketStatus.Open
        };

        Assert.NotEqual(Guid.Empty, ticket.Id);
        Assert.Equal(assetId, ticket.AssetId);
        Assert.Equal(MaintenanceTicketStatus.Open, ticket.Status);
    }
}
