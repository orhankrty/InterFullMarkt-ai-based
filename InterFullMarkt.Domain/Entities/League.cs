namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Lig (Liqa) Aggregate Root.
/// İçinde klubları, seviyesi ve uluslararası katsayılarını barındırır.
/// Rich Domain Model prensiplerine uygun.
/// </summary>
public sealed class League : BaseEntity, IAuditEntity, ISoftDelete
{
    /// <summary>
    /// Ligin adı (İspanyol Ligi, Bundesliga, vb.)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Ligin bulunduğu ülke
    /// </summary>
    public required Nationality Country { get; set; }

    /// <summary>
    /// Ligin seviyesi (1 = En yüksek lig, 2 = İkinci lig, vb.)
    /// </summary>
    public required int Tier { get; set; }

    /// <summary>
    /// UEFA/Uluslararası başarı katsayısı (0.0 - 10.0)
    /// </summary>
    public required decimal Coefficient { get; set; }

    /// <summary>
    /// Ligin slogan veya açıklaması
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ligin telefon ya da logo PNG URL'si
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Bu lige ait kulüpler
    /// </summary>
    public virtual ICollection<Club> Clubs { get; private set; } = new List<Club>();

    // IAuditEntity implementasyon
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    // ISoftDelete implementasyon
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    private League() { }

    /// <summary>
    /// Yeni lig oluşturur
    /// </summary>
    /// <param name="name">Lig adı</param>
    /// <param name="country">Lig bulunduğu ülke</param>
    /// <param name="tier">Lig seviyesi</param>
    /// <param name="coefficient">Uluslararası katsayısı</param>
    public League(
        string name,
        Nationality country,
        int tier,
        decimal coefficient) : base(Guid.NewGuid())
    {
        Name = Protect.Against.NullOrEmpty(name, nameof(name));
        Country = Protect.Against.Null(country, nameof(country));
        Tier = Protect.Against.Negative(tier, nameof(tier));

        if (coefficient < 0 || coefficient > 10)
            throw new ArgumentException("Katsayı 0.0 ile 10.0 arasında olmalıdır", nameof(coefficient));

        Coefficient = coefficient;
    }

    /// <summary>
    /// Lig önemini belirtir (Coefficient'e göre)
    /// </summary>
    public string GetTierDescription()
        => Tier switch
        {
            1 => "Top-1 Lig",
            2 => "İkinci Seviye Lig",
            3 => "Üçüncü Seviye Lig",
            _ => $"Tier {Tier}"
        };

    /// <summary>
    /// Katsayıyı günceller (finansal performans değişikliği)
    /// </summary>
    public void UpdateCoefficient(decimal newCoefficient)
    {
        if (newCoefficient < 0 || newCoefficient > 10)
            throw new ArgumentException("Katsayı 0.0 ile 10.0 arasında olmalıdır", nameof(newCoefficient));

        Coefficient = newCoefficient;
        UpdateTimestamp();
    }

    /// <summary>
    /// Ligin ne kadar prestijli olduğunu belirtir
    /// </summary>
    public bool IsPremierLeague => Tier == 1 && Coefficient >= 7.0m;

    public void Delete(string deletedByUserId)
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        DeletedByUserId = Protect.Against.NullOrEmpty(deletedByUserId, nameof(deletedByUserId));
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedDate = null;
        DeletedByUserId = null;
    }
}
