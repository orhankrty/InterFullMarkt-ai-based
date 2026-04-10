namespace InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;
using InterFullMarkt.Application.Abstractions;

/// <summary>
/// CreatePlayerCommand'ı işleyen handler.
/// Validasyon, Entity oluşturma, veri tabanına kaydetme işlemleri burada yapılır.
/// </summary>
public sealed class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, CreatePlayerResult>
{
    private readonly IDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext">Veritabanı konteksti</param>
    public CreatePlayerCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// Yeni oyuncu oluşturur ve veri tabanına kaydeder.
    /// </summary>
    /// <param name="request">Komut isteği</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>İşlem sonucu</returns>
    public async Task<CreatePlayerResult> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. DTO'dan Entity alanları çıkar
            var playerData = request.PlayerData;

            // 2. Milliyeti bulur
            var nationality = Nationality.CreateByCode(playerData.NationalityCode);
            if (nationality is null)
                return CreatePlayerResult.FailureResult(
                    $"'{playerData.NationalityCode}' kodu için milliyeti bulunamadı.",
                    "INVALID_NATIONALITY");

            // 3. Pozisyon Enum'ını dönüştür
            if (!Enum.IsDefined(typeof(PlayerPosition), playerData.Position))
                return CreatePlayerResult.FailureResult(
                    "Geçersiz pozisyon değeri.",
                    "INVALID_POSITION");

            var position = (PlayerPosition)playerData.Position;

            // 4. Player Entity'sini oluştur
            var player = new Player(
                fullName: playerData.FullName,
                position: position,
                nationality: nationality,
                dateOfBirth: playerData.DateOfBirth,
                height: playerData.Height,
                weight: playerData.Weight)
            {
                PreferredFoot = playerData.PreferredFoot,
                JerseyNumber = playerData.JerseyNumber,
                CreatedByUserId = request.CreatedByUserId
            };

            // 5. Piyasa değeri varsa ayarla
            if (playerData.InitialMarketValue.HasValue && playerData.InitialMarketValue.Value > 0)
            {
                player.UpdateMarketValue(
                    Money.Create(playerData.InitialMarketValue.Value, playerData.Currency));
            }

            // 6. Eğer kulüp ID'si belirtilmişse, kulübün mevcut olup olmadığını kontrol et
            if (playerData.CurrentClubId.HasValue)
            {
                var club = await _dbContext.Clubs
                    .FirstOrDefaultAsync(c => c.Id == playerData.CurrentClubId.Value, cancellationToken);

                if (club == null)
                    return CreatePlayerResult.FailureResult(
                        $"ID'si {playerData.CurrentClubId} olan kulüp bulunamadı.",
                        "CLUB_NOT_FOUND");

                // Kulübün kadro limitini kontrol et (23 oyuncu max)
                var squadCount = club.Players.Count;
                if (squadCount >= 23)
                    return CreatePlayerResult.FailureResult(
                        $"{club.Name} kadrosu dolu (Max: 23 oyuncu).",
                        "SQUAD_FULL");
                
                // Kulüp atanıyor
                player.CurrentClubId = playerData.CurrentClubId;
            }

            // 7. Veri tabanına ekle
            _dbContext.Add(player);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 8. Başarı sonucu döndür
            return CreatePlayerResult.SuccessResult(
                player.Id,
                $"'{playerData.FullName}' başarılı bir şekilde sisteme eklendi.");
        }
        catch (Exception ex)
        {
            return CreatePlayerResult.FailureResult(
                $"Oyuncu oluşturulurken bir hata oluştu: {ex.Message}",
                "SYSTEM_ERROR");
        }
    }
}
