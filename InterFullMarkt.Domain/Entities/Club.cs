namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Futbol Kulübü (Club) Aggregate Root.
/// Bir kulübün bütçesi, kadrosu, stadyumu ve renkleri gibi temel bilgilerini tutar.
/// Rich Domain Model: AddPlayer, RemovePlayer metodları ile kadro yönetimini yapar.
/// </summary>
public sealed class Club : BaseEntity, IAuditEntity, ISoftDelete
{
    private const int MaxSquadSize = 23;

    /// <summary>
    /// Kulübün tam adı (Manchester United Football Club)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Kulübün kısa adı (MAN)
    /// </summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>
    /// Kulübün kuruluş yılı
    /// </summary>
    public int FoundingYear { get; set; }

    /// <summary>
    /// Stadyumun adı
    /// </summary>
    public string StadiumName { get; set; } = string.Empty;

    /// <summary>
    /// Kulüp renkleri (virgülle ayrılmış, örn: "Red,White")
    /// </summary>
    public string Colors { get; set; } = string.Empty;

    /// <summary>
    /// Ligin ID'si
    /// </summary>
    public Guid LeagueId { get; set; }

    /// <summary>
    /// Ligin navigasyon özelliği
    /// </summary>
    public League? League { get; set; }

    /// <summary>
    /// Kulübün bütçesi
    /// </summary>
    public Money? Budget { get; set; }

    /// <summary>
    /// Kulüp kurulu futbolcular
    /// </summary>
    public ICollection<Player> Players { get; set; } = new List<Player>();

    /// <summary>
    /// Kulübün gönderdiği transferler (FromClub olarak)
    /// </summary>
    public ICollection<Transfer> OutgoingTransfers { get; set; } = new List<Transfer>();

    /// <summary>
    /// Kulübün aldığı transferler (ToClub olarak)
    /// </summary>
    public ICollection<Transfer> IncomingTransfers { get; set; } = new List<Transfer>();

    /// <summary>
    /// Kulüp logosu URL'si
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// Kulübün açıklaması veya tarihi
    /// </summary>
    public string? Description { get; set; }

    // IAuditEntity implementasyon
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    // ISoftDelete implementasyon
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    private Club() { }

    /// <summary>
    /// Yeni kulüp oluşturur
    /// </summary>
    public Club(
        string name,
        string shortName,
        int foundingYear,
        string stadiumName,
        string colors,
        Guid leagueId,
        Money? initialBudget = null) : base(Guid.NewGuid())
    {
        Name = Protect.Against.NullOrEmpty(name, nameof(name));
        ShortName = Protect.Against.NullOrEmpty(shortName, nameof(shortName));
        FoundingYear = Protect.Against.Negative(foundingYear, nameof(foundingYear));
        StadiumName = Protect.Against.NullOrEmpty(stadiumName, nameof(stadiumName));
        Colors = Protect.Against.NullOrEmpty(colors, nameof(colors));
        LeagueId = leagueId != Guid.Empty ? leagueId : throw new ArgumentException("Lig ID'si geçerli olmalıdır", nameof(leagueId));

        Budget = initialBudget;
    }

    /// <summary>
    /// Kulübün yaşını (kuruluş yılından itibaren) hesaplar
    /// </summary>
    public int GetAge()
        => DateTime.UtcNow.Year - FoundingYear;

    /// <summary>
    /// Kadrotaki futbolcu sayısını döndürür
    /// </summary>
    public int GetSquadCount()
        => Players.Count(p => !p.IsDeleted);

    /// <summary>
    /// Kadra yeni oyuncu ekler (Maximum 23 oyuncu)
    /// </summary>
    /// <param name="player">Eklenecek oyuncu</param>
    /// <exception cref="InvalidOperationException">Kadro doluysa veya oyuncu başka kulüpteyse</exception>
    public void AddPlayer(Player player)
    {
        Protect.Against.Null(player, nameof(player));

        if (GetSquadCount() >= MaxSquadSize)
            throw new InvalidOperationException($"Kadro dolu! Maksimum {MaxSquadSize} oyuncu barındırılamaz.");

        if (player.CurrentClubId.HasValue && player.CurrentClubId != Id)
            throw new InvalidOperationException("Bu oyuncu başka bir kulüpte oynuyor!");

        player.TransferToClub(Id);
        Players.Add(player);
        UpdateTimestamp();
    }

    /// <summary>
    /// Kadrodan oyuncu çıkarır (transfer veya satış)
    /// </summary>
    /// <param name="player">Çıkarılacak oyuncu</param>
    /// <exception cref="InvalidOperationException">Oyuncu bu kulüpte değilse</exception>
    public void RemovePlayer(Player player)
    {
        Protect.Against.Null(player, nameof(player));

        if (player.CurrentClubId != Id)
            throw new InvalidOperationException("Bu oyuncu bu kulüpte oynamıyor!");

        player.RemoveFromClub();
        Players.Remove(player);
        UpdateTimestamp();
    }

    /// <summary>
    /// Kulübün bütçesini günceller
    /// </summary>
    public void UpdateBudget(Money newBudget)
    {
        Protect.Against.Null(newBudget, nameof(newBudget));
        Budget = newBudget;
        UpdateTimestamp();
    }

    /// <summary>
    /// Bütçeden transfer ücretini düşer
    /// </summary>
    /// <param name="transferFee">Transfer ücreti</param>
    /// <exception cref="InvalidOperationException">Bütçe yetersizse</exception>
    public void DeductFromBudget(Money transferFee)
    {
        Protect.Against.Null(transferFee, nameof(transferFee));

        if (Budget is null)
            throw new InvalidOperationException("Kulübün bütçesi tanımlanmamış!");

        if (transferFee > Budget!)
            throw new InvalidOperationException($"Bütçe yetersiz! Kulüp bütçesi: {Budget}, Transfer ücreti: {transferFee}");

        Budget = Budget.Subtract(transferFee);
    }

    /// <summary>
    /// Bütçeye transfer geliri ekler
    /// </summary>
    public void AddToBudget(Money amount)
    {
        Protect.Against.Null(amount, nameof(amount));

        Budget ??= Money.Zero(amount.Currency);
        Budget = Budget.Add(amount);
    }

    /// <summary>
    /// Kadrodaki toplam oyuncu pazarlama değerini hesaplar
    /// </summary>
    public Money? CalculateTotalSquadValue()
    {
        var activePlayers = Players.Where(p => !p.IsDeleted && p.MarketValue is not null).ToList();

        if (!activePlayers.Any())
            return null;

        var firstCurrency = activePlayers.First().MarketValue?.Currency ?? "EUR";
        var totalValue = activePlayers
            .Where(p => p.MarketValue?.Currency == firstCurrency)
            .Aggregate(Money.Zero(firstCurrency), (acc, p) => acc.Add(p.MarketValue!));

        return totalValue;
    }

    /// <summary>
    /// Kulüpü soft delete eder
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

    /// <summary>
    /// Kulübün aktif olup olmadığını belirtir
    /// </summary>
    public bool IsActive => !IsDeleted;
}
