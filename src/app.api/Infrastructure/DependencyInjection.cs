using System.Globalization;
using app.api.Infrastructure.Migrations.PostgreSQL;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace app.api.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("CoreDb");
        Console.WriteLine($"connectionString->coredb: {connectionString}");
        Guard.Against.Null(connectionString, message: "Connection string 'CoreDb' not found.");

        builder.Services
            .AddDatabase(connectionString);

    }


    private static IServiceCollection AddDatabase(this IServiceCollection services, string dbConnectionString)
    {

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<OutboxMessagesInterceptor>();
            options
                // .UseNpgsql(builder.Configuration.GetConnectionString("CoreDb"))
                // options.EnableSensitiveDataLogging()
                .UseNpgsql(
                    dbConnectionString,
                    npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(IMigrationBase).Assembly)
                )
                .UseSnakeCaseNamingConvention(CultureInfo.CurrentCulture)
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                .AddInterceptors(interceptor)
                ;
        });

        services.AddScoped<DatabaseInitialiser>();


        return services;
    }

    public static void UseInfrastructureForTesting(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            return;
        }

        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<DatabaseInitialiser>();

        // run database building
        Task.Run(async () =>
        {
            // Start both awaitable methods in parallel
            var task1 = initialiser.InitialiseAsync();
            var task2 = initialiser.SeedAsync();

            // Await all tasks to complete
            await Task.WhenAll(task1, task2);

            initialiser.DropDatabaseOnFinish(app);
        }).GetAwaiter().GetResult();
    }
}

internal sealed class DatabaseInitialiser(
    ILogger<DatabaseInitialiser> logger,
    ApplicationDbContext dbContext
)
{
    internal async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            // await ApplyMigrations();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    internal void DropDatabaseOnFinish(WebApplication app)
    {

        var db = dbContext.Database;
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            Console.WriteLine("Dropping database...");
            db.EnsureDeletedAsync().GetAwaiter().GetResult();
        });
    }

    private async Task ApplyMigrationsAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Console.WriteLine("Info: Applying pending migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Info: Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("Info: No pending migrations found.");
        }

    }

    internal async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        logger.LogDebug("No data to seed");
        await Task.Delay(100);
    }
}
