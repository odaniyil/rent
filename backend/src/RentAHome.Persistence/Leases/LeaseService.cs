using Microsoft.EntityFrameworkCore;
using RentAHome.Application.Leases;
using RentAHome.Domain.Entities;
using RentAHome.Domain.Enums;

namespace RentAHome.Persistence.Leases;

public sealed class LeaseService(RentAHomeDbContext dbContext) : ILeaseService
{
    public async Task<IReadOnlyList<OwnerLeaseResponse>> ListOwnerLeasesAsync(
        OwnerLeaseListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = OwnerLeasesQuery();

        if (request.OwnerId.HasValue)
        {
            query = query.Where(lease => lease.OwnerId == request.OwnerId.Value);
        }

        if (request.PropertyId.HasValue)
        {
            query = query.Where(lease => lease.PropertyId == request.PropertyId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(lease => lease.Status == request.Status.Value);
        }

        var leases = await query
            .OrderByDescending(lease => lease.StartDate)
            .ToListAsync(cancellationToken);

        return leases.Select(MapOwnerLease).ToList();
    }

    public async Task<OwnerLeaseResponse?> GetOwnerLeaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lease = await OwnerLeasesQuery()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return lease is null ? null : MapOwnerLease(lease);
    }

    public async Task<LeaseOperationResult<OwnerLeaseResponse>> CreateOwnerLeaseAsync(
        CreateOwnerLeaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateOwnerLeaseAsync(request, cancellationToken);
        if (validationError is not null)
        {
            return LeaseOperationResult<OwnerLeaseResponse>.Failure(validationError);
        }

        var lease = new OwnerLease
        {
            OwnerId = request.OwnerId,
            PropertyId = request.PropertyId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyRent = request.MonthlyRent,
            SecurityDeposit = request.SecurityDeposit,
            SubleaseAllowed = request.SubleaseAllowed,
            Status = request.Status
        };

        dbContext.OwnerLeases.Add(lease);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetOwnerLeaseAsync(lease.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created owner lease could not be loaded.");

        return LeaseOperationResult<OwnerLeaseResponse>.Success(response);
    }

    public async Task<LeaseOperationResult<OwnerLeaseResponse>> UpdateOwnerLeaseAsync(
        Guid id,
        UpdateOwnerLeaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var lease = await dbContext.OwnerLeases
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (lease is null)
        {
            return LeaseOperationResult<OwnerLeaseResponse>.Failure("Owner lease was not found.");
        }

        var validationError = await ValidateOwnerLeaseAsync(request, cancellationToken);
        if (validationError is not null)
        {
            return LeaseOperationResult<OwnerLeaseResponse>.Failure(validationError);
        }

        lease.OwnerId = request.OwnerId;
        lease.PropertyId = request.PropertyId;
        lease.StartDate = request.StartDate;
        lease.EndDate = request.EndDate;
        lease.MonthlyRent = request.MonthlyRent;
        lease.SecurityDeposit = request.SecurityDeposit;
        lease.SubleaseAllowed = request.SubleaseAllowed;
        lease.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetOwnerLeaseAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Updated owner lease could not be loaded.");

        return LeaseOperationResult<OwnerLeaseResponse>.Success(response);
    }

    public async Task<bool> DeleteOwnerLeaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lease = await dbContext.OwnerLeases
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (lease is null)
        {
            return false;
        }

        dbContext.OwnerLeases.Remove(lease);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<TenantLeaseResponse>> ListTenantLeasesAsync(
        TenantLeaseListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = TenantLeasesQuery();

        if (request.TenantId.HasValue)
        {
            query = query.Where(lease => lease.TenantId == request.TenantId.Value);
        }

        if (request.PropertyId.HasValue)
        {
            query = query.Where(lease => lease.PropertyId == request.PropertyId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(lease => lease.Status == request.Status.Value);
        }

        var leases = await query
            .OrderByDescending(lease => lease.StartDate)
            .ToListAsync(cancellationToken);

        return leases.Select(MapTenantLease).ToList();
    }

    public async Task<TenantLeaseResponse?> GetTenantLeaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lease = await TenantLeasesQuery()
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        return lease is null ? null : MapTenantLease(lease);
    }

    public async Task<LeaseOperationResult<TenantLeaseResponse>> CreateTenantLeaseAsync(
        CreateTenantLeaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationError = await ValidateTenantLeaseAsync(request, null, cancellationToken);
        if (validationError is not null)
        {
            return LeaseOperationResult<TenantLeaseResponse>.Failure(validationError);
        }

        var lease = new TenantLease
        {
            TenantId = request.TenantId,
            PropertyId = request.PropertyId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyRent = request.MonthlyRent,
            SecurityDeposit = request.SecurityDeposit,
            SecurityDepositReceived = request.SecurityDepositReceived,
            Status = request.Status
        };

        dbContext.TenantLeases.Add(lease);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetTenantLeaseAsync(lease.Id, cancellationToken)
            ?? throw new InvalidOperationException("Created tenant lease could not be loaded.");

        return LeaseOperationResult<TenantLeaseResponse>.Success(response);
    }

    public async Task<LeaseOperationResult<TenantLeaseResponse>> UpdateTenantLeaseAsync(
        Guid id,
        UpdateTenantLeaseRequest request,
        CancellationToken cancellationToken = default)
    {
        var lease = await dbContext.TenantLeases
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (lease is null)
        {
            return LeaseOperationResult<TenantLeaseResponse>.Failure("Tenant lease was not found.");
        }

        var validationError = await ValidateTenantLeaseAsync(request, id, cancellationToken);
        if (validationError is not null)
        {
            return LeaseOperationResult<TenantLeaseResponse>.Failure(validationError);
        }

        lease.TenantId = request.TenantId;
        lease.PropertyId = request.PropertyId;
        lease.StartDate = request.StartDate;
        lease.EndDate = request.EndDate;
        lease.MonthlyRent = request.MonthlyRent;
        lease.SecurityDeposit = request.SecurityDeposit;
        lease.SecurityDepositReceived = request.SecurityDepositReceived;
        lease.Status = request.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetTenantLeaseAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Updated tenant lease could not be loaded.");

        return LeaseOperationResult<TenantLeaseResponse>.Success(response);
    }

    public async Task<bool> DeleteTenantLeaseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lease = await dbContext.TenantLeases
            .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

        if (lease is null)
        {
            return false;
        }

        dbContext.TenantLeases.Remove(lease);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<string?> ValidateOwnerLeaseAsync(CreateOwnerLeaseRequest request, CancellationToken cancellationToken)
    {
        return await ValidateOwnerLeaseFieldsAsync(
            request.OwnerId,
            request.PropertyId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.SecurityDeposit,
            cancellationToken);
    }

    private async Task<string?> ValidateOwnerLeaseAsync(UpdateOwnerLeaseRequest request, CancellationToken cancellationToken)
    {
        return await ValidateOwnerLeaseFieldsAsync(
            request.OwnerId,
            request.PropertyId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.SecurityDeposit,
            cancellationToken);
    }

    private async Task<string?> ValidateOwnerLeaseFieldsAsync(
        Guid ownerId,
        Guid propertyId,
        DateOnly startDate,
        DateOnly endDate,
        decimal monthlyRent,
        decimal securityDeposit,
        CancellationToken cancellationToken)
    {
        if (ownerId == Guid.Empty)
        {
            return "Owner id is required.";
        }

        if (propertyId == Guid.Empty)
        {
            return "Property id is required.";
        }

        if (endDate < startDate)
        {
            return "Lease end date must be on or after the start date.";
        }

        if (monthlyRent <= 0)
        {
            return "Monthly rent must be greater than zero.";
        }

        if (securityDeposit < 0)
        {
            return "Security deposit cannot be negative.";
        }

        var ownerExists = await dbContext.Owners.AnyAsync(owner => owner.Id == ownerId, cancellationToken);
        if (!ownerExists)
        {
            return "Owner was not found.";
        }

        var propertyExists = await dbContext.Properties
            .AnyAsync(property => property.Id == propertyId && property.DeletedAtUtc == null, cancellationToken);
        if (!propertyExists)
        {
            return "Property was not found.";
        }

        return null;
    }

    private async Task<string?> ValidateTenantLeaseAsync(
        CreateTenantLeaseRequest request,
        Guid? currentLeaseId,
        CancellationToken cancellationToken)
    {
        return await ValidateTenantLeaseFieldsAsync(
            request.TenantId,
            request.PropertyId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.SecurityDeposit,
            request.SecurityDepositReceived,
            request.Status,
            currentLeaseId,
            cancellationToken);
    }

    private async Task<string?> ValidateTenantLeaseAsync(
        UpdateTenantLeaseRequest request,
        Guid? currentLeaseId,
        CancellationToken cancellationToken)
    {
        return await ValidateTenantLeaseFieldsAsync(
            request.TenantId,
            request.PropertyId,
            request.StartDate,
            request.EndDate,
            request.MonthlyRent,
            request.SecurityDeposit,
            request.SecurityDepositReceived,
            request.Status,
            currentLeaseId,
            cancellationToken);
    }

    private async Task<string?> ValidateTenantLeaseFieldsAsync(
        Guid tenantId,
        Guid propertyId,
        DateOnly startDate,
        DateOnly endDate,
        decimal monthlyRent,
        decimal securityDeposit,
        bool securityDepositReceived,
        TenantLeaseStatus status,
        Guid? currentLeaseId,
        CancellationToken cancellationToken)
    {
        if (tenantId == Guid.Empty)
        {
            return "Tenant id is required.";
        }

        if (propertyId == Guid.Empty)
        {
            return "Property id is required.";
        }

        if (endDate < startDate)
        {
            return "Lease end date must be on or after the start date.";
        }

        if (monthlyRent <= 0)
        {
            return "Monthly rent must be greater than zero.";
        }

        if (securityDeposit < 0)
        {
            return "Security deposit cannot be negative.";
        }

        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.Id == tenantId, cancellationToken);
        if (!tenantExists)
        {
            return "Tenant was not found.";
        }

        var propertyExists = await dbContext.Properties
            .AnyAsync(property => property.Id == propertyId && property.DeletedAtUtc == null, cancellationToken);
        if (!propertyExists)
        {
            return "Property was not found.";
        }

        var subleaseAllowed = await dbContext.OwnerLeases.AnyAsync(
            lease => lease.PropertyId == propertyId
                && lease.SubleaseAllowed
                && (lease.Status == OwnerLeaseStatus.Signed || lease.Status == OwnerLeaseStatus.Active),
            cancellationToken);
        if (!subleaseAllowed)
        {
            return "Tenant lease cannot be created until a signed owner lease allows subleasing.";
        }

        if (status == TenantLeaseStatus.Active && !securityDepositReceived)
        {
            return "Tenant lease cannot be activated until security deposit is marked received.";
        }

        if (status == TenantLeaseStatus.Active)
        {
            var hasActiveTenantLease = await dbContext.TenantLeases.AnyAsync(
                lease => lease.PropertyId == propertyId
                    && lease.Status == TenantLeaseStatus.Active
                    && (!currentLeaseId.HasValue || lease.Id != currentLeaseId.Value),
                cancellationToken);
            if (hasActiveTenantLease)
            {
                return "Property already has an active tenant lease.";
            }
        }

        return null;
    }

    private IQueryable<OwnerLease> OwnerLeasesQuery()
    {
        return dbContext.OwnerLeases
            .AsNoTracking()
            .Include(lease => lease.Owner)
            .Include(lease => lease.Property);
    }

    private IQueryable<TenantLease> TenantLeasesQuery()
    {
        return dbContext.TenantLeases
            .AsNoTracking()
            .Include(lease => lease.Tenant)
            .Include(lease => lease.Property);
    }

    private static OwnerLeaseResponse MapOwnerLease(OwnerLease lease)
    {
        return new OwnerLeaseResponse(
            lease.Id,
            lease.OwnerId,
            lease.Owner?.FullName,
            lease.PropertyId,
            lease.Property?.Name,
            lease.StartDate,
            lease.EndDate,
            lease.MonthlyRent,
            lease.SecurityDeposit,
            lease.SubleaseAllowed,
            lease.Status);
    }

    private static TenantLeaseResponse MapTenantLease(TenantLease lease)
    {
        return new TenantLeaseResponse(
            lease.Id,
            lease.TenantId,
            lease.Tenant?.FullName,
            lease.PropertyId,
            lease.Property?.Name,
            lease.StartDate,
            lease.EndDate,
            lease.MonthlyRent,
            lease.SecurityDeposit,
            lease.SecurityDepositReceived,
            lease.Status);
    }
}
