using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RentAHome.Application.Auth;
using RentAHome.Application.Leases;
using RentAHome.Application.Properties;
using RentAHome.Domain.Enums;
using RentAHome.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddPersistence(builder.Configuration);

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey))
{
    throw new InvalidOperationException("JWT signing key is not configured.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health")
    .WithName("HealthCheck");

app.MapGet("/", () => Results.Ok(new
{
    name = "Rent-A-Home API",
    status = "Ready"
}))
    .WithName("Root");

var auth = app.MapGroup("/api/auth")
    .WithTags("Auth");

auth.MapPost("/login", async (LoginRequest request, IAuthService authService, CancellationToken cancellationToken) =>
{
    var response = await authService.LoginAsync(request, cancellationToken);
    return response is null ? Results.Unauthorized() : Results.Ok(response);
})
    .AllowAnonymous()
    .WithName("Login");

var test = app.MapGroup("/api/test")
    .WithTags("Auth Test")
    .RequireAuthorization();

test.MapGet("/protected", (ClaimsPrincipal user) => Results.Ok(new
{
    message = "Authenticated",
    user = user.Identity?.Name
}))
    .WithName("ProtectedTest");

test.MapGet("/super-admin", (ClaimsPrincipal user) => Results.Ok(new
{
    message = "SuperAdmin access granted",
    user = user.Identity?.Name
}))
    .RequireAuthorization(policy => policy.RequireRole(UserRole.SuperAdmin.ToString()))
    .WithName("SuperAdminTest");

var properties = app.MapGroup("/api/properties")
    .WithTags("Properties")
    .RequireAuthorization();

properties.MapGet("/", async (
    string? city,
    string? locality,
    PropertyStatus? status,
    string? occupancy,
    DateOnly? leaseExpiringBefore,
    IPropertyService propertyService,
    CancellationToken cancellationToken) =>
{
    var request = new PropertyListRequest(city, locality, status, occupancy, leaseExpiringBefore);
    var response = await propertyService.ListAsync(request, cancellationToken);
    return Results.Ok(response);
})
    .WithName("ListProperties");

properties.MapGet("/{id:guid}", async (
    Guid id,
    IPropertyService propertyService,
    CancellationToken cancellationToken) =>
{
    var response = await propertyService.GetDetailAsync(id, cancellationToken);
    return response is null ? Results.NotFound() : Results.Ok(response);
})
    .WithName("GetPropertyDetail");

properties.MapPost("/", async (
    CreatePropertyRequest request,
    IPropertyService propertyService,
    CancellationToken cancellationToken) =>
{
    var validationError = ValidatePropertyRequest(
        request.Name,
        request.AddressLine1,
        request.City,
        request.State,
        request.PostalCode,
        request.OwnerId);

    if (validationError is not null)
    {
        return Results.BadRequest(new { error = validationError });
    }

    try
    {
        var response = await propertyService.CreateAsync(request, cancellationToken);
        return Results.Created($"/api/properties/{response.Id}", response);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
    .WithName("CreateProperty");

properties.MapPut("/{id:guid}", async (
    Guid id,
    UpdatePropertyRequest request,
    IPropertyService propertyService,
    CancellationToken cancellationToken) =>
{
    var validationError = ValidatePropertyRequest(
        request.Name,
        request.AddressLine1,
        request.City,
        request.State,
        request.PostalCode,
        request.OwnerId);

    if (validationError is not null)
    {
        return Results.BadRequest(new { error = validationError });
    }

    try
    {
        var response = await propertyService.UpdateAsync(id, request, cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
    .WithName("UpdateProperty");

properties.MapDelete("/{id:guid}", async (
    Guid id,
    IPropertyService propertyService,
    CancellationToken cancellationToken) =>
{
    var deleted = await propertyService.SoftDeleteAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
})
    .WithName("DeleteProperty");

var ownerLeases = app.MapGroup("/api/owner-leases")
    .WithTags("Owner Leases")
    .RequireAuthorization();

ownerLeases.MapGet("/", async (
    Guid? ownerId,
    Guid? propertyId,
    OwnerLeaseStatus? status,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var request = new OwnerLeaseListRequest(ownerId, propertyId, status);
    var response = await leaseService.ListOwnerLeasesAsync(request, cancellationToken);
    return Results.Ok(response);
})
    .WithName("ListOwnerLeases");

ownerLeases.MapGet("/{id:guid}", async (
    Guid id,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.GetOwnerLeaseAsync(id, cancellationToken);
    return response is null ? Results.NotFound() : Results.Ok(response);
})
    .WithName("GetOwnerLease");

ownerLeases.MapPost("/", async (
    CreateOwnerLeaseRequest request,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.CreateOwnerLeaseAsync(request, cancellationToken);
    return response.Succeeded
        ? Results.Created($"/api/owner-leases/{response.Value!.Id}", response.Value)
        : Results.BadRequest(new { error = response.Error });
})
    .WithName("CreateOwnerLease");

ownerLeases.MapPut("/{id:guid}", async (
    Guid id,
    UpdateOwnerLeaseRequest request,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.UpdateOwnerLeaseAsync(id, request, cancellationToken);
    return ToMutationResult(response);
})
    .WithName("UpdateOwnerLease");

ownerLeases.MapDelete("/{id:guid}", async (
    Guid id,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var deleted = await leaseService.DeleteOwnerLeaseAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
})
    .WithName("DeleteOwnerLease");

var tenantLeases = app.MapGroup("/api/tenant-leases")
    .WithTags("Tenant Leases")
    .RequireAuthorization();

tenantLeases.MapGet("/", async (
    Guid? tenantId,
    Guid? propertyId,
    TenantLeaseStatus? status,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var request = new TenantLeaseListRequest(tenantId, propertyId, status);
    var response = await leaseService.ListTenantLeasesAsync(request, cancellationToken);
    return Results.Ok(response);
})
    .WithName("ListTenantLeases");

tenantLeases.MapGet("/{id:guid}", async (
    Guid id,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.GetTenantLeaseAsync(id, cancellationToken);
    return response is null ? Results.NotFound() : Results.Ok(response);
})
    .WithName("GetTenantLease");

tenantLeases.MapPost("/", async (
    CreateTenantLeaseRequest request,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.CreateTenantLeaseAsync(request, cancellationToken);
    return response.Succeeded
        ? Results.Created($"/api/tenant-leases/{response.Value!.Id}", response.Value)
        : Results.BadRequest(new { error = response.Error });
})
    .WithName("CreateTenantLease");

tenantLeases.MapPut("/{id:guid}", async (
    Guid id,
    UpdateTenantLeaseRequest request,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var response = await leaseService.UpdateTenantLeaseAsync(id, request, cancellationToken);
    return ToMutationResult(response);
})
    .WithName("UpdateTenantLease");

tenantLeases.MapDelete("/{id:guid}", async (
    Guid id,
    ILeaseService leaseService,
    CancellationToken cancellationToken) =>
{
    var deleted = await leaseService.DeleteTenantLeaseAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
})
    .WithName("DeleteTenantLease");

app.Run();

static IResult ToMutationResult<T>(LeaseOperationResult<T> result)
{
    if (result.Succeeded)
    {
        return Results.Ok(result.Value);
    }

    if (result.Error?.EndsWith("was not found.", StringComparison.OrdinalIgnoreCase) == true)
    {
        return Results.NotFound(new { error = result.Error });
    }

    return Results.BadRequest(new { error = result.Error });
}

static string? ValidatePropertyRequest(
    string name,
    string addressLine1,
    string city,
    string state,
    string postalCode,
    Guid ownerId)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return "Property name is required.";
    }

    if (string.IsNullOrWhiteSpace(addressLine1))
    {
        return "Address line 1 is required.";
    }

    if (string.IsNullOrWhiteSpace(city))
    {
        return "City is required.";
    }

    if (string.IsNullOrWhiteSpace(state))
    {
        return "State is required.";
    }

    if (string.IsNullOrWhiteSpace(postalCode))
    {
        return "Postal code is required.";
    }

    if (ownerId == Guid.Empty)
    {
        return "Owner id is required.";
    }

    return null;
}
