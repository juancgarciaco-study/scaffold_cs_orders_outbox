using app.api.Domain;
using app.api.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;

namespace app.api.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Ignore<IHasDomainEvents>();

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Ulid>()
            // for guid db type
            .HaveConversion<UlidToGuidConverter>()
            // for varchar db type
            // .HaveConversion<UlidToStringConverter>()
            // for binary db type
            // .HaveConversion<UlidToBytesConverter>()
            ;
    }
}
