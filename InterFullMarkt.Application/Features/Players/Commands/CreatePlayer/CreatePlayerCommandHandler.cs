namespace InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;

using MediatR;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public CreatePlayerCommandHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Yeni oyuncu oluşturur ve veri tabanına kaydeder.
    /// </summary>
    public async Task<CreatePlayerResult> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. DTO'dan Entity alanları çıkar
            var playerData = request.PlayerData;

            // 2. Milliyeti bulur
            var nationality = Nationality.GetByCode(playerData.NationalityCode);
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
                CurrentClubId = playerData.CurrentClubId,
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
                var club = await _dbContext.Clubs.FindAsync(
                    new object?[] { playerData.CurrentClubId.Value }, 
                    cancellationToken: cancellationToken);

                if (club == null)
                    return CreatePlayerResult.FailureResult(
                        $"ID'si {playerData.CurrentClubId} olan kulüp bulunamadı.",
                        "CLUB_NOT_FOUND");

                // Kulübün kadro limitini kontrol et
                var squadCount = club.GetSquadCount();
                if (squadCount >= 23)
                    return CreatePlayerResult.FailureResult(
                        $"{club.Name} kadrosu dolu (Max: 23 oyuncu).",
                        "SQUAD_FULL");
            }

            // 7. Veri tabanına ekle
            await _dbContext.Players.AddAsync(player, cancellationToken);
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
