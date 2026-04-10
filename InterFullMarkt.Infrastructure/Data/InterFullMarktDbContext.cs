namespace InterFullMarkt.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Infrastructure.Data.Configurations;
using InterFullMarkt.Infrastructure.Data.Seeds;

/// <summary>
/// InterFullMarkt veritabanı konteksti.
/// .NET 10.0 Primary Constructor ile yazılmış, Fluent API ile yapılandırılmış.
/// Global Query Filter ile Soft Delete otomasyonu sağlar.
/// </summary>
public sealed class InterFullMarktDbContext(DbContextOptions<InterFullMarktDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Futbolcular tablosu
    /// </summary>
    public required DbSet<Player> Players { get; set; }

    /// <summary>
    /// Kulüpler tablosu
    /// </summary>
    public required DbSet<Club> Clubs { get; set; }

    /// <summary>
    /// Ligler tablosu
    /// </summary>
    public required DbSet<League> Leagues { get; set; }

    /// <summary>
    /// Transferler tablosu
    /// </summary>
    public required DbSet<Transfer> Transfers { get; set; }

    /// <summary>
    /// DbContext konfigürasyonu:
    /// - Entity Type Configurations
    /// - Global Query Filters (Soft Delete)
    /// - Seed Data
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity Configurations
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new ClubConfiguration());
        modelBuilder.ApplyConfiguration(new LeagueConfiguration());
        modelBuilder.ApplyConfiguration(new TransferConfiguration());

        // Global Query Filters
        ApplyGlobalQueryFilters(modelBuilder);

        // Seed Data
        SeedData.ConfigureSeedData(modelBuilder);
    }

    /// <summary>
    /// ISoftDelete arayüzüne sahip tüm varlıklar için Global Query Filter uygular.
    /// Silinmiş (IsDeleted = true) veriler otomatikman filtrelenir.
    /// </summary>
    private static void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        var softDeleteEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(et => typeof(ISoftDelete).IsAssignableFrom(et.ClrType))
            .ToList();

        foreach (var entityType in softDeleteEntities)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType);
            var filterExpression = System.Linq.Expressions.Expression.Lambda(
                System.Linq.Expressions.Expression.Equal(
                    System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                    System.Linq.Expressions.Expression.Constant(false, typeof(bool))),
                parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filterExpression);
        }
    }

    /// <summary>
    /// SaveChanges öncesinde Audit bilgilerini doldurur.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditableEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// SaveChangesAsync öncesinde Audit bilgilerini doldurur.
    /// </summary>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>
    /// IAuditEntity arayüzüne sahip varlıkların audit bilgilerini otomatik doldurur.
    /// </summary>
    private void UpdateAuditableEntities()
    {
        var auditableEntities = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified))
            .ToList();

        foreach (var entry in auditableEntities)
        {
            if (entry.Entity is not IAuditEntity auditEntity)
                continue;

            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                auditEntity.CreatedDate = now;
                auditEntity.UpdatedDate = now;
                auditEntity.CreatedByUserId ??= "System";
            }

            if (entry.State == EntityState.Modified)
            {
                auditEntity.UpdatedDate = now;
                auditEntity.UpdatedByUserId ??= "System";
            }
        }
    }
}
