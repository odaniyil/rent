using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RentAHome.Application.Auth;
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

    var response = await propertyService.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/properties/{response.Id}", response);
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

    var response = await propertyService.UpdateAsync(id, request, cancellationToken);
    return response is null ? Results.NotFound() : Results.Ok(response);
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

app.Run();

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
