namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Futbolcu Aggregate Root.
/// Her bir futbolcunun biyometrik, finansal ve kariyer verilerini tutar.
/// Rich Domain Model prensiplerine uygun, kendi geçerliliğini kontrol eder.
/// </summary>
public sealed class Player : BaseEntity, IAuditEntity, ISoftDelete
{
    /// <summary>
    /// Futbolcunun adı ve soyadı (Required)
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Futbolcunun mevkii (Required)
    /// </summary>
    public required PlayerPosition Position { get; set; }

    /// <summary>
    /// Milliyeti (Required)
    /// </summary>
    public required Nationality Nationality { get; set; }

    /// <summary>
    /// Doğum tarihi
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Boy (cm cinsinden)
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Kilo (kg cinsinden)
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Tercih edilen ayak (sol/sağ)
    /// </summary>
    public string PreferredFoot { get; set; } = "Right";

    /// <summary>
    /// Mevcut piyasa değeri
    /// </summary>
    public Money? MarketValue { get; private set; }

    /// <summary>
    /// Forması tutuğu numara
    /// </summary>
    public int? JerseyNumber { get; set; }

    /// <summary>
    /// Bulunduğu kulüp ID'si
    /// </summary>
    public Guid? CurrentClubId { get; private set; }

    // Navigation properties removed - configured in EF Core

    // IAuditEntity implementasyon
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    // ISoftDelete implementasyon
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    private Player() { }

    /// <summary>
    /// Yeni futbolcu oluşturur
    /// </summary>
    public Player(
        string fullName,
        PlayerPosition position,
        Nationality nationality,
        DateTime dateOfBirth,
        int height,
        decimal weight) : base(Guid.NewGuid())
    {
        FullName = Protect.Against.NullOrEmpty(fullName, nameof(fullName));
        Position = position;
        Nationality = Protect.Against.Null(nationality, nameof(nationality));
        DateOfBirth = Protect.Against.InTheFuture(dateOfBirth, nameof(dateOfBirth));
        Height = Protect.Against.Negative(height, nameof(height));
        Weight = Protect.Against.NegativeOrZero(weight, nameof(weight));
    }

    /// <summary>
    /// Futbolcunun yaşını hesaplar
    /// </summary>
    public int GetAge()
    {
        var today = DateTime.UtcNow;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }

    /// <summary>
    /// Futbolcunun piyasa değerini günceller
    /// </summary>
    /// <param name="newValue">Yeni piyasa değeri</param>
    public void UpdateMarketValue(Money newValue)
    {
        Protect.Against.Null(newValue, nameof(newValue));

        MarketValue = newValue;
        UpdateTimestamp();
    }

    /// <summary>
    /// Futbolcuyu bir kulübe transfer eder
    /// </summary>
    /// <param name="clubId">Hedef kulüp ID'si</param>
    public void TransferToClub(Guid clubId)
    {
        if (clubId == Guid.Empty)
            throw new ArgumentException("Kulüp ID'si geçerli olmalıdır", nameof(clubId));

        CurrentClubId = clubId;
        UpdateTimestamp();
    }

    /// <summary>
    /// Futbolcuyu kulüpten çıkarır (transfer veya hürriyet)
    /// </summary>
    public void RemoveFromClub()
    {
        CurrentClubId = null;
        UpdateTimestamp();
    }

    /// <summary>
    /// Futbolcuyu sağlıklı/sakatlandırır
    /// </summary>
    public bool IsActive => !IsDeleted && CurrentClubId.HasValue;

    /// <summary>
    /// Futbolcuyu soft delete eder
    /// </summary>
    public void Delete(string deletedByUserId)
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
        DeletedByUserId = Protect.Against.NullOrEmpty(deletedByUserId, nameof(deletedByUserId));
    }

    /// <summary>
    /// Soft delete'i geri alır
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedDate = null;
        DeletedByUserId = null;
    }
}
