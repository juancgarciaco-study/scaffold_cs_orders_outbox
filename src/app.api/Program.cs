using app.api.Infrastructure;
using app.api.ServiceExtensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();

builder.AddInfrastructureServices();

builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseInfrastructureForTesting();

// Map all other feature endpoints automatically
app.MapFeatureEndpoints();

await app.RunAsync();
