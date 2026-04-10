namespace InterFullMarkt.Application.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using InterFullMarkt.Domain.Entities;

/// <summary>
/// Database konteksti abstraksiyon interface'i.    /// Clean Architecture prensiplerine uygun olarak DbContext'i abstraktlaştırır.
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Futbolcular DbSet'i
    /// </summary>
    DbSet<Player> Players { get; }

    /// <summary>
    /// Kulüpler DbSet'i
    /// </summary>
    DbSet<Club> Clubs { get; }

    /// <summary>
    /// Ligler DbSet'i
    /// </summary>
    DbSet<League> Leagues { get; }

    /// <summary>
    /// Transferler DbSet'i
    /// </summary>
    DbSet<Transfer> Transfers { get; }

    /// <summary>
    /// Varlık ekler
    /// </summary>
    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Değişiklikleri veritabanına kaydeder.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

