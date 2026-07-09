using Microsoft.EntityFrameworkCore;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;

namespace RentAHome.Persistence;

public sealed class RentAHomeDbContext(DbContextOptions<RentAHomeDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Owner> Owners => Set<Owner>();
    public DbSet<OwnerLease> OwnerLeases => Set<OwnerLease>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantLease> TenantLeases => Set<TenantLease>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<MaintenanceTicket> MaintenanceTickets => Set<MaintenanceTicket>();
    public DbSet<RentSchedule> RentSchedules => Set<RentSchedule>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Warranty> Warranties => Set<Warranty>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureOwners(modelBuilder);
        ConfigureProperties(modelBuilder);
        ConfigureOwnerLeases(modelBuilder);
        ConfigureTenants(modelBuilder);
        ConfigureTenantLeases(modelBuilder);
        ConfigureRooms(modelBuilder);
        ConfigureAssets(modelBuilder);
        ConfigureMaintenanceTickets(modelBuilder);
        ConfigureRentSchedules(modelBuilder);
        ConfigureDocuments(modelBuilder);
        ConfigureVendors(modelBuilder);
        ConfigureWarranties(modelBuilder);
        ConfigureInsurancePolicies(modelBuilder);
        ConfigureInspections(modelBuilder);
        ConfigureAuditLogs(modelBuilder);
        ConfigureAuthUsers(modelBuilder);
        SeedData(modelBuilder);
    }

    private static void ConfigureOwners(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(entity =>
        {
            entity.Property(owner => owner.FullName).HasMaxLength(200);
            entity.Property(owner => owner.Email).HasMaxLength(320);
            entity.Property(owner => owner.PhoneNumber).HasMaxLength(50);
            entity.Property(owner => owner.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void ConfigureProperties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Property>(entity =>
        {
            entity.Property(property => property.Name).HasMaxLength(200);
            entity.Property(property => property.AddressLine1).HasMaxLength(300);
            entity.Property(property => property.AddressLine2).HasMaxLength(300);
            entity.Property(property => property.Locality).HasMaxLength(120);
            entity.Property(property => property.City).HasMaxLength(120);
            entity.Property(property => property.State).HasMaxLength(120);
            entity.Property(property => property.PostalCode).HasMaxLength(30);
            entity.Property(property => property.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(property => property.Owner)
                .WithMany(owner => owner.Properties)
                .HasForeignKey(property => property.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureOwnerLeases(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OwnerLease>(entity =>
        {
            entity.Property(lease => lease.MonthlyRent).HasPrecision(18, 2);
            entity.Property(lease => lease.SecurityDeposit).HasPrecision(18, 2);
            entity.Property(lease => lease.SubleaseAllowed).HasDefaultValue(false);
            entity.Property(lease => lease.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(lease => lease.Owner)
                .WithMany(owner => owner.OwnerLeases)
                .HasForeignKey(lease => lease.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lease => lease.Property)
                .WithMany(property => property.OwnerLeases)
                .HasForeignKey(lease => lease.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureTenants(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.Property(tenant => tenant.FullName).HasMaxLength(200);
            entity.Property(tenant => tenant.Email).HasMaxLength(320);
            entity.Property(tenant => tenant.PhoneNumber).HasMaxLength(50);
            entity.Property(tenant => tenant.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void ConfigureTenantLeases(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TenantLease>(entity =>
        {
            entity.Property(lease => lease.MonthlyRent).HasPrecision(18, 2);
            entity.Property(lease => lease.SecurityDeposit).HasPrecision(18, 2);
            entity.Property(lease => lease.SecurityDepositReceived).HasDefaultValue(false);
            entity.Property(lease => lease.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(lease => lease.Tenant)
                .WithMany(tenant => tenant.TenantLeases)
                .HasForeignKey(lease => lease.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(lease => lease.Property)
                .WithMany(property => property.TenantLeases)
                .HasForeignKey(lease => lease.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureRooms(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(room => room.Name).HasMaxLength(120);
            entity.Property(room => room.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(room => room.Property)
                .WithMany(property => property.Rooms)
                .HasForeignKey(room => room.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureAssets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.Property(asset => asset.Name).HasMaxLength(200);
            entity.Property(asset => asset.Category).HasMaxLength(120);
            entity.Property(asset => asset.SerialNumber).HasMaxLength(120);
            entity.Property(asset => asset.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(asset => asset.Property)
                .WithMany(property => property.Assets)
                .HasForeignKey(asset => asset.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(asset => asset.Room)
                .WithMany(room => room.Assets)
                .HasForeignKey(asset => asset.RoomId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureMaintenanceTickets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaintenanceTicket>(entity =>
        {
            entity.Property(ticket => ticket.Title).HasMaxLength(200);
            entity.Property(ticket => ticket.Description).HasMaxLength(2000);
            entity.Property(ticket => ticket.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(ticket => ticket.Property)
                .WithMany(property => property.MaintenanceTickets)
                .HasForeignKey(ticket => ticket.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ticket => ticket.Tenant)
                .WithMany(tenant => tenant.MaintenanceTickets)
                .HasForeignKey(ticket => ticket.TenantId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(ticket => ticket.Asset)
                .WithMany(asset => asset.MaintenanceTickets)
                .HasForeignKey(ticket => ticket.AssetId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(ticket => ticket.Vendor)
                .WithMany(vendor => vendor.MaintenanceTickets)
                .HasForeignKey(ticket => ticket.VendorId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureRentSchedules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RentSchedule>(entity =>
        {
            entity.Property(schedule => schedule.Amount).HasPrecision(18, 2);
            entity.Property(schedule => schedule.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(schedule => schedule.TenantLease)
                .WithMany(lease => lease.RentSchedules)
                .HasForeignKey(schedule => schedule.TenantLeaseId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureDocuments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.Property(document => document.FileName).HasMaxLength(255);
            entity.Property(document => document.StoragePath).HasMaxLength(1000);
            entity.Property(document => document.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void ConfigureVendors(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.Property(vendor => vendor.Name).HasMaxLength(200);
            entity.Property(vendor => vendor.ContactName).HasMaxLength(200);
            entity.Property(vendor => vendor.Email).HasMaxLength(320);
            entity.Property(vendor => vendor.PhoneNumber).HasMaxLength(50);
            entity.Property(vendor => vendor.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void ConfigureWarranties(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Warranty>(entity =>
        {
            entity.Property(warranty => warranty.ProviderName).HasMaxLength(200);
            entity.Property(warranty => warranty.PolicyNumber).HasMaxLength(120);
            entity.Property(warranty => warranty.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(warranty => warranty.Asset)
                .WithMany(asset => asset.Warranties)
                .HasForeignKey(warranty => warranty.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureInsurancePolicies(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InsurancePolicy>(entity =>
        {
            entity.Property(policy => policy.ProviderName).HasMaxLength(200);
            entity.Property(policy => policy.PolicyNumber).HasMaxLength(120);
            entity.Property(policy => policy.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(policy => policy.Property)
                .WithMany(property => property.InsurancePolicies)
                .HasForeignKey(policy => policy.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureInspections(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inspection>(entity =>
        {
            entity.Property(inspection => inspection.Notes).HasMaxLength(2000);
            entity.Property(inspection => inspection.Status).HasConversion<string>().HasMaxLength(50);

            entity.HasOne(inspection => inspection.Property)
                .WithMany(property => property.Inspections)
                .HasForeignKey(inspection => inspection.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureAuditLogs(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(log => log.EntityName).HasMaxLength(200);
            entity.Property(log => log.EntityId).HasMaxLength(120);
            entity.Property(log => log.Action).HasMaxLength(100);
            entity.Property(log => log.CreatedBy).HasMaxLength(200);
            entity.Property(log => log.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void ConfigureAuthUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthUser>(entity =>
        {
            entity.Property(user => user.Email).HasMaxLength(320);
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.PasswordHash).HasMaxLength(500);
            entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(80);
            entity.Property(user => user.Status).HasConversion<string>().HasMaxLength(50);
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var ownerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var propertyId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var tenantId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var livingRoomId = Guid.Parse("40000000-0000-0000-0000-000000000001");
        var bedroomId = Guid.Parse("40000000-0000-0000-0000-000000000002");
        var sofaId = Guid.Parse("50000000-0000-0000-0000-000000000001");
        var bedId = Guid.Parse("50000000-0000-0000-0000-000000000002");
        var ownerLeaseId = Guid.Parse("60000000-0000-0000-0000-000000000001");
        var tenantLeaseId = Guid.Parse("70000000-0000-0000-0000-000000000001");
        var superAdminId = Guid.Parse("80000000-0000-0000-0000-000000000001");
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Owner>().HasData(new Owner
        {
            Id = ownerId,
            FullName = "Demo Owner",
            Email = "owner@example.com",
            PhoneNumber = "+1-555-0100",
            Status = OwnerStatus.Active,
            CreatedAtUtc = createdAt
        });

        modelBuilder.Entity<Property>().HasData(new Property
        {
            Id = propertyId,
            OwnerId = ownerId,
            Name = "Demo Apartment",
            AddressLine1 = "100 Market Street",
            Locality = "SoMa",
            City = "San Francisco",
            State = "CA",
            PostalCode = "94105",
            Status = PropertyStatus.Leased,
            CreatedAtUtc = createdAt
        });

        modelBuilder.Entity<Tenant>().HasData(new Tenant
        {
            Id = tenantId,
            FullName = "Demo Tenant",
            Email = "tenant@example.com",
            PhoneNumber = "+1-555-0200",
            Status = TenantStatus.Active,
            CreatedAtUtc = createdAt
        });

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = livingRoomId,
                PropertyId = propertyId,
                Name = "Living Room",
                Status = RoomStatus.Active
            },
            new Room
            {
                Id = bedroomId,
                PropertyId = propertyId,
                Name = "Bedroom",
                Status = RoomStatus.Active
            });

        modelBuilder.Entity<Asset>().HasData(
            new Asset
            {
                Id = sofaId,
                PropertyId = propertyId,
                RoomId = livingRoomId,
                Name = "Sofa",
                Category = "Furniture",
                PurchaseDate = new DateOnly(2025, 12, 15),
                Status = AssetStatus.Installed
            },
            new Asset
            {
                Id = bedId,
                PropertyId = propertyId,
                RoomId = bedroomId,
                Name = "Queen Bed",
                Category = "Furniture",
                PurchaseDate = new DateOnly(2025, 12, 15),
                Status = AssetStatus.Installed
            });

        modelBuilder.Entity<OwnerLease>().HasData(new OwnerLease
        {
            Id = ownerLeaseId,
            OwnerId = ownerId,
            PropertyId = propertyId,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31),
            MonthlyRent = 2500m,
            SecurityDeposit = 2500m,
            SubleaseAllowed = true,
            Status = OwnerLeaseStatus.Active
        });

        modelBuilder.Entity<TenantLease>().HasData(new TenantLease
        {
            Id = tenantLeaseId,
            TenantId = tenantId,
            PropertyId = propertyId,
            StartDate = new DateOnly(2026, 2, 1),
            EndDate = new DateOnly(2027, 1, 31),
            MonthlyRent = 3200m,
            SecurityDeposit = 3200m,
            SecurityDepositReceived = true,
            Status = TenantLeaseStatus.Active
        });

        modelBuilder.Entity<AuthUser>().HasData(new AuthUser
        {
            Id = superAdminId,
            Email = "superadmin@rentahome.local",
            PasswordHash = "pbkdf2_sha256$100000$cmVudC1hLWhvbWUtZGVtby1zdXBlcmFkbWluLXNhbHQ=$q9OkGlrDjiENYIp50z1Vfq4GFx7weRi/qX1Bg9ki/nI=",
            Role = UserRole.SuperAdmin,
            Status = UserAccountStatus.Active,
            CreatedAtUtc = createdAt
        });
    }
}
