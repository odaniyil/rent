namespace RentAHome.Application.Leases;

public sealed record LeaseOperationResult<T>(T? Value, string? Error)
{
    public bool Succeeded => Error is null;

    public static LeaseOperationResult<T> Success(T value) => new(value, null);

    public static LeaseOperationResult<T> Failure(string error) => new(default, error);
}
