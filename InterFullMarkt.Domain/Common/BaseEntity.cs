namespace InterFullMarkt.Domain.Common;

/// <summary>
/// Tüm domain varlıklarının temelini oluşturan abstract sınıf.
/// Generic TId ile farklı ID tiplerini destekler (int, Guid, long, etc.)
/// </summary>
/// <typeparam name="TId">Varlığın ID tipi</typeparam>
public abstract class BaseEntity<TId> where TId : notnull
{
    /// <summary>
    /// Varlık kimliği
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Varlığın oluşturulma tarihi (UTC)
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Varlığın güncellenme tarihi (UTC)
    /// </summary>
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    protected BaseEntity() { }

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Varlığın güncellenme tarihini current UTC zamanına ayarlar
    /// </summary>
    public void UpdateTimestamp() => UpdatedDate = DateTime.UtcNow;
}

/// <summary>
/// Default olarak Guid kullanacak varlıklar için convenience sınıfı
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity() : base() { }

    protected BaseEntity(Guid id) : base(id) { }
}
