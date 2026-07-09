using RentAHome.Domain.Enums;

namespace RentAHome.Application.Properties;

public sealed record CreatePropertyRequest(
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string City,
    string State,
    string PostalCode,
    Guid OwnerId,
    PropertyStatus Status);

public sealed record UpdatePropertyRequest(
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string City,
    string State,
    string PostalCode,
    Guid OwnerId,
    PropertyStatus Status);

public sealed record PropertyListRequest(
    string? City,
    string? Locality,
    PropertyStatus? Status,
    string? Occupancy,
    DateOnly? LeaseExpiringBefore);

public sealed record PropertyListItemResponse(
    Guid Id,
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string City,
    string State,
    string PostalCode,
    PropertyStatus Status,
    Guid OwnerId,
    string? OwnerName,
    string Occupancy,
    DateOnly? LeaseExpiry);

public sealed record LeaseSummaryResponse(
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MonthlyRent,
    string Status);

public sealed record ProfitabilitySummaryResponse(
    decimal MonthlyOwnerRent,
    decimal MonthlyTenantRent,
    decimal EstimatedMonthlyGrossProfit,
    string Notes);

public sealed record PropertyDetailResponse(
    Guid Id,
    string Name,
    string AddressLine1,
    string? AddressLine2,
    string? Locality,
    string City,
    string State,
    string PostalCode,
    PropertyStatus Status,
    Guid OwnerId,
    string? OwnerName,
    string Occupancy,
    LeaseSummaryResponse? OwnerLease,
    LeaseSummaryResponse? TenantLease,
    int InventoryCount,
    int MaintenanceCount,
    ProfitabilitySummaryResponse ProfitabilitySummary);
