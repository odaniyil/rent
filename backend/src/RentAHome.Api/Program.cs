var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health")
    .WithName("HealthCheck");

app.MapGet("/", () => Results.Ok(new
{
    name = "Rent-A-Home API",
    status = "Ready"
}))
    .WithName("Root");

app.Run();
