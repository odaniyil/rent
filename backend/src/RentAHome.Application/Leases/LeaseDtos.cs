using RentAHome.Domain.Enums;

namespace RentAHome.Application.Leases;

public sealed record CreateOwnerLeaseRequest(
    Guid OwnerId,
    Guid PropertyId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SubleaseAllowed,
    OwnerLeaseStatus Status);

public sealed record UpdateOwnerLeaseRequest(
    Guid OwnerId,
    Guid PropertyId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SubleaseAllowed,
    OwnerLeaseStatus Status);

public sealed record OwnerLeaseListRequest(
    Guid? OwnerId,
    Guid? PropertyId,
    OwnerLeaseStatus? Status);

public sealed record OwnerLeaseResponse(
    Guid Id,
    Guid OwnerId,
    string? OwnerName,
    Guid PropertyId,
    string? PropertyName,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SubleaseAllowed,
    OwnerLeaseStatus Status);

public sealed record CreateTenantLeaseRequest(
    Guid TenantId,
    Guid PropertyId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SecurityDepositReceived,
    TenantLeaseStatus Status);

public sealed record UpdateTenantLeaseRequest(
    Guid TenantId,
    Guid PropertyId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SecurityDepositReceived,
    TenantLeaseStatus Status);

public sealed record TenantLeaseListRequest(
    Guid? TenantId,
    Guid? PropertyId,
    TenantLeaseStatus? Status);

public sealed record TenantLeaseResponse(
    Guid Id,
    Guid TenantId,
    string? TenantName,
    Guid PropertyId,
    string? PropertyName,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    decimal SecurityDeposit,
    bool SecurityDepositReceived,
    TenantLeaseStatus Status);
