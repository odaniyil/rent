using Microsoft.EntityFrameworkCore;
using RentAHome.Application.Properties;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;

namespace RentAHome.Persistence.Properties;

public sealed class PropertyService(RentAHomeDbContext dbContext) : IPropertyService
{
    public async Task<IReadOnlyList<PropertyListItemResponse>> ListAsync(
        PropertyListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BasePropertiesQuery();

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToUpperInvariant();
            query = query.Where(property => property.City.ToUpper() == city);
        }

        if (!string.IsNullOrWhiteSpace(request.Locality))
        {
            var locality = request.Locality.Trim().ToUpperInvariant();
            query = query.Where(property => property.Locality != null && property.Locality.ToUpper() == locality);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(property => property.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Occupancy))
        {
            var occupancy = request.Occupancy.Trim();
            if (occupancy.Equals("Occupied", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(property => property.TenantLeases.Any(lease => lease.Status == TenantLeaseStatus.Active));
            }
            else if (occupancy.Equals("Vacant", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(property => !property.TenantLeases.Any(lease => lease.Status == TenantLeaseStatus.Active));
            }
        }

        if (request.LeaseExpiringBefore.HasValue)
        {
            query = query.Where(property => property.TenantLeases
                .Any(lease => lease.Status == TenantLeaseStatus.Active && lease.EndDate <= request.LeaseExpiringBefore.Value));
        }

        var properties = await query
            .OrderBy(property => property.Name)
            .ToListAsync(cancellationToken);

        return properties.Select(MapListItem).ToList();
    }

    public async Task<PropertyDetailResponse?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var property = await BasePropertiesQuery()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return property is null ? null : MapDetail(property);
    }

    public async Task<PropertyDetailResponse> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Status == PropertyStatus.SetupInProgress)
        {
            throw new InvalidOperationException(
                "Property cannot move to SetupInProgress without a signed owner lease.");
        }

        var property = new Property
        {
            Name = request.Name.Trim(),
            AddressLine1 = request.AddressLine1.Trim(),
            AddressLine2 = NormalizeOptional(request.AddressLine2),
            Locality = NormalizeOptional(request.Locality),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            OwnerId = request.OwnerId,
            Status = request.Status,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Properties.Add(property);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetDetailAsync(property.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created property could not be loaded.");
    }

    public async Task<PropertyDetailResponse?> UpdateAsync(
        Guid id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default)
    {
        var property = await dbContext.Properties
            .SingleOrDefaultAsync(candidate => candidate.Id == id && candidate.DeletedAtUtc == null, cancellationToken);

        if (property is null)
        {
            return null;
        }

        if (request.Status == PropertyStatus.SetupInProgress)
        {
            var hasSignedOwnerLease = await dbContext.OwnerLeases.AnyAsync(
                lease => lease.PropertyId == id
                    && (lease.Status == OwnerLeaseStatus.Signed || lease.Status == OwnerLeaseStatus.Active),
                cancellationToken);
            if (!hasSignedOwnerLease)
            {
                throw new InvalidOperationException(
                    "Property cannot move to SetupInProgress without a signed owner lease.");
            }
        }

        property.Name = request.Name.Trim();
        property.AddressLine1 = request.AddressLine1.Trim();
        property.AddressLine2 = NormalizeOptional(request.AddressLine2);
        property.Locality = NormalizeOptional(request.Locality);
        property.City = request.City.Trim();
        property.State = request.State.Trim();
        property.PostalCode = request.PostalCode.Trim();
        property.OwnerId = request.OwnerId;
        property.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetDetailAsync(id, cancellationToken);
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var property = await dbContext.Properties
            .SingleOrDefaultAsync(candidate => candidate.Id == id && candidate.DeletedAtUtc == null, cancellationToken);

        if (property is null)
        {
            return false;
        }

        property.Status = PropertyStatus.Inactive;
        property.DeletedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Property> BasePropertiesQuery()
    {
        return dbContext.Properties
            .AsNoTracking()
            .Include(property => property.Owner)
            .Include(property => property.OwnerLeases)
            .Include(property => property.TenantLeases)
            .Include(property => property.Assets)
            .Include(property => property.MaintenanceTickets)
            .Where(property => property.DeletedAtUtc == null);
    }

    private static PropertyListItemResponse MapListItem(Property property)
    {
        var activeTenantLease = property.TenantLeases
            .Where(lease => lease.Status == TenantLeaseStatus.Active)
            .OrderByDescending(lease => lease.EndDate)
            .FirstOrDefault();

        return new PropertyListItemResponse(
            property.Id,
            property.Name,
            property.AddressLine1,
            property.AddressLine2,
            property.Locality,
            property.City,
            property.State,
            property.PostalCode,
            property.Status,
            property.OwnerId,
            property.Owner?.FullName,
            activeTenantLease is null ? "Vacant" : "Occupied",
            activeTenantLease?.EndDate);
    }

    private static PropertyDetailResponse MapDetail(Property property)
    {
        var ownerLease = property.OwnerLeases
            .Where(lease => lease.Status == OwnerLeaseStatus.Active)
            .OrderByDescending(lease => lease.EndDate)
            .FirstOrDefault();
        var tenantLease = property.TenantLeases
            .Where(lease => lease.Status == TenantLeaseStatus.Active)
            .OrderByDescending(lease => lease.EndDate)
            .FirstOrDefault();

        var ownerRent = ownerLease?.MonthlyRent ?? 0m;
        var tenantRent = tenantLease?.MonthlyRent ?? 0m;

        return new PropertyDetailResponse(
            property.Id,
            property.Name,
            property.AddressLine1,
            property.AddressLine2,
            property.Locality,
            property.City,
            property.State,
            property.PostalCode,
            property.Status,
            property.OwnerId,
            property.Owner?.FullName,
            tenantLease is null ? "Vacant" : "Occupied",
            ownerLease is null ? null : new LeaseSummaryResponse(
                ownerLease.Id,
                ownerLease.StartDate,
                ownerLease.EndDate,
                ownerLease.MonthlyRent,
                ownerLease.Status.ToString()),
            tenantLease is null ? null : new LeaseSummaryResponse(
                tenantLease.Id,
                tenantLease.StartDate,
                tenantLease.EndDate,
                tenantLease.MonthlyRent,
                tenantLease.Status.ToString()),
            property.Assets.Count,
            property.MaintenanceTickets.Count,
            new ProfitabilitySummaryResponse(
                ownerRent,
                tenantRent,
                tenantRent - ownerRent,
                "Placeholder summary based on current active lease rents only."));
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
