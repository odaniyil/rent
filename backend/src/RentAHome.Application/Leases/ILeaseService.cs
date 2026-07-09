namespace RentAHome.Application.Leases;

public interface ILeaseService
{
    Task<IReadOnlyList<OwnerLeaseResponse>> ListOwnerLeasesAsync(
        OwnerLeaseListRequest request,
        CancellationToken cancellationToken = default);

    Task<OwnerLeaseResponse?> GetOwnerLeaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeaseOperationResult<OwnerLeaseResponse>> CreateOwnerLeaseAsync(
        CreateOwnerLeaseRequest request,
        CancellationToken cancellationToken = default);

    Task<LeaseOperationResult<OwnerLeaseResponse>> UpdateOwnerLeaseAsync(
        Guid id,
        UpdateOwnerLeaseRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteOwnerLeaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TenantLeaseResponse>> ListTenantLeasesAsync(
        TenantLeaseListRequest request,
        CancellationToken cancellationToken = default);

    Task<TenantLeaseResponse?> GetTenantLeaseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<LeaseOperationResult<TenantLeaseResponse>> CreateTenantLeaseAsync(
        CreateTenantLeaseRequest request,
        CancellationToken cancellationToken = default);

    Task<LeaseOperationResult<TenantLeaseResponse>> UpdateTenantLeaseAsync(
        Guid id,
        UpdateTenantLeaseRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteTenantLeaseAsync(Guid id, CancellationToken cancellationToken = default);
}
