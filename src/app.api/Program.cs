using app.api.Infrastructure;
using app.api.ServiceExtensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<OutboxMessagesInterceptor>();

builder.AddInfrastructureServices();
/*
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>();
    var sqlConnectionString = builder.Configuration.GetConnectionString("CoreDb");
    options
        // .UseNpgsql(builder.Configuration.GetConnectionString("CoreDb"))
        // options.EnableSensitiveDataLogging()
        .UseNpgsql(
            sqlConnectionString,
            npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(IMigrationBase).Assembly)
        )
        .UseSnakeCaseNamingConvention(CultureInfo.CurrentCulture)
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
        .AddInterceptors(interceptor)
        ;
});
*/
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
