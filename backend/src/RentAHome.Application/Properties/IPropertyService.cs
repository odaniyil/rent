namespace RentAHome.Application.Properties;

public interface IPropertyService
{
    Task<IReadOnlyList<PropertyListItemResponse>> ListAsync(
        PropertyListRequest request,
        CancellationToken cancellationToken = default);

    Task<PropertyDetailResponse?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PropertyDetailResponse> CreateAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<PropertyDetailResponse?> UpdateAsync(
        Guid id,
        UpdatePropertyRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
