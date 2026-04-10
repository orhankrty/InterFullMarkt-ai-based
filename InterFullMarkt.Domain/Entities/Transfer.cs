namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Transfer varlığı - Projenin en kritik 'Transaction' noktasıdır.
/// Bir futbolcunun bir kulüpten diğer kulüpe transferini temsil eder.
/// Business Logic: Transfer gerçekleşebilmesi için güçlü validasyonlar içerir.
/// </summary>
public sealed class Transfer : BaseEntity, IAuditEntity, ISoftDelete
{
    /// <summary>
    /// Transferi gönderen (eski) kulüp ID'si
    /// </summary>
    public required Guid FromClubId { get; set; }

    /// <summary>
    /// Transferi alan (yeni) kulüp ID'si
    /// </summary>
    public required Guid ToClubId { get; set; }

    /// <summary>
    /// Transfer edilen futbolcu ID'si
    /// </summary>
    public required Guid PlayerId { get; set; }

    /// <summary>
    /// Transfer ücreti (Bedel)
    /// </summary>
    public required Money Fee { get; set; }

    /// <summary>
    /// Transferin gerçekleşme tarihi
    /// </summary>
    public required DateTime TransferDate { get; set; }

    /// <summary>
    /// Transfer tipi (Free, Loan, Permanent, vb.)
    /// </summary>
    public required string TransferType { get; set; } = "Permanent";

    /// <summary>
    /// Transfer açıklaması veya notları
    /// </summary>
    public string? Notes { get; set; }

    // IAuditEntity implementasyon
    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    // ISoftDelete implementasyon
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    private Transfer() { }

    /// <summary>
    /// Yeni transfer oluşturur
    /// </summary>
    /// <param name="fromClubId">Gönderen kulüp</param>
    /// <param name="toClubId">Alan kulüp</param>
    /// <param name="playerId">Transfer edilen oyuncu</param>
    /// <param name="fee">Transfer ücreti</param>
    /// <param name="transferDate">Transfer tarihi</param>
    /// <param name="transferType">Transfer tipi</param>
    public Transfer(
        Guid fromClubId,
        Guid toClubId,
        Guid playerId,
        Money fee,
        DateTime transferDate,
        string transferType = "Permanent") : base(Guid.NewGuid())
    {
        // Validasyonlar: Bağımlılık Kontrolleri
        if (fromClubId == Guid.Empty)
            throw new ArgumentException("Gönderen kulüp ID'si geçerli olmalıdır", nameof(fromClubId));

        if (toClubId == Guid.Empty)
            throw new ArgumentException("Alan kulüp ID'si geçerli olmalıdır", nameof(toClubId));

        if (fromClubId == toClubId)
            throw new InvalidOperationException("Aynı kulüp içerisinde transfer yapılamaz!");

        if (playerId == Guid.Empty)
            throw new ArgumentException("Oyuncu ID'si geçerli olmalıdır", nameof(playerId));

        Protect.Against.Null(fee, nameof(fee));

        // Tarih doğrulaması: Transfer tarihi geçmiş veya bugün olmalı
        if (transferDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Transfer tarihi gelecekte olamaz", nameof(transferDate));

        Protect.Against.NullOrEmpty(transferType, nameof(transferType));

        FromClubId = fromClubId;
        ToClubId = toClubId;
        PlayerId = playerId;
        Fee = fee;
        TransferDate = transferDate;
        TransferType = transferType;
    }

    /// <summary>
    /// Transfer öncesinde eksiksiz validasyon yapar
    /// </summary>
    /// <param name="player">Transfer edilecek oyuncu</param>
    /// <param name="fromClub">Gönderen kulüp</param>
    /// <param name="toClub">Alan kulüp</param>
    /// <exception cref="InvalidOperationException">Validasyon başarısız olursa</exception>
    public void ValidateTransfer(Player player, Club fromClub, Club toClub)
    {
        Protect.Against.Null(player, nameof(player));
        Protect.Against.Null(fromClub, nameof(fromClub));
        Protect.Against.Null(toClub, nameof(toClub));

        // 1. Oyuncu şu anda gönderen kulüpte mi?
        if (player.CurrentClubId != FromClubId)
            throw new InvalidOperationException($"Oyuncu {fromClub.Name}'de oynamıyor!");

        // 2. Oyuncu silinmiş mi?
        if (player.IsDeleted)
            throw new InvalidOperationException("Transfer yapılacak oyuncu silinmiş!");

        // 3. Alan kulüp kadrosu doluysa?
        if (toClub.GetSquadCount() >= 23)
            throw new InvalidOperationException($"{toClub.Name} kadrosu dolu!");

        // 4. Alan kulübün bütçesi yeterli mi?
        if (toClub.Budget != null && Fee > toClub.Budget)
            throw new InvalidOperationException($"{toClub.Name} bütçesi yetersiz! Tersiyorum: {Fee}, Mevcut: {toClub.Budget}");

        // 5. Transfer ücreti negatif mi?
        if (Fee.Amount < 0)
            throw new InvalidOperationException("Transfer ücreti negatif olamaz!");
    }

    /// <summary>
    /// Transferi tamamlar (oyuncu ve kulüpleri günceller)
    /// </summary>
    public void CompleteTransfer(Player player, Club fromClub, Club toClub)
    {
        ValidateTransfer(player, fromClub, toClub);

        // 1. Gönderen kulüpten bi oyuncu çıkar
        fromClub.RemovePlayer(player);

        // 2. Alan kulübe oyuncu ekle
        toClub.AddPlayer(player);

        // 3. Bütçeleri güncelle
        if (Fee.Amount > 0)
        {
            toClub.DeductFromBudget(Fee);
            fromClub.AddToBudget(Fee);
        }

        UpdateTimestamp();
    }

    /// <summary>
    /// Transfer bilgisini string olarak döndürür
    /// </summary>
    public override string ToString()
        => $"{FromClubId} -> {ToClubId}: {PlayerId} ({Fee})";

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
