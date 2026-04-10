namespace InterFullMarkt.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using InterFullMarkt.Domain;

/// <summary>
/// Entity Framework Core SaveChanges Interceptor'ı.
/// IAuditEntity arayüzüne sahip varlıkların CreatedDate, UpdatedDate ve kullanıcı bilgilerini otomatik doldurur.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// SaveChanges çağrılmadan önce Audit bilgilerini günceller.
    /// </summary>
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// SaveChangesAsync çağrılmadan önce Audit bilgilerini günceller.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// IAuditEntity arayüzüne sahip varlıkları bulur ve audit alanlarını doldurur.
    /// </summary>
    private static void UpdateAuditableEntities(DbContext context)
    {
        var auditableEntities = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAuditEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified))
            .ToList();

        var now = DateTime.UtcNow;
        var currentUser = GetCurrentUser();

        foreach (var entry in auditableEntities)
        {
            if (entry.Entity is not IAuditEntity auditEntity)
                continue;

            if (entry.State == EntityState.Added)
            {
                auditEntity.CreatedDate = now;
                auditEntity.UpdatedDate = now;
                auditEntity.CreatedByUserId = currentUser;
            }

            if (entry.State == EntityState.Modified)
            {
                auditEntity.UpdatedDate = now;
                auditEntity.UpdatedByUserId = currentUser;
            }
        }
    }

    /// <summary>
    /// Mevcut kullanıcıyı bulur (şu an "System" döndürüyor, sonra HttpContext'ten alınabilir).
    /// </summary>
    private static string GetCurrentUser()
    {
        // TODO: HttpContext'ten veya Claims'den user ID'sini al
        // var user = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // return user ?? "Anonymous";

        return "System";
    }
}
