namespace InterFullMarkt.Application.Features.Players.Commands.CreatePlayer;

using MediatR;
using InterFullMarkt.Application.DTOs;

/// <summary>
/// Yeni oyuncu oluşturma komutu (MediatR Command).
/// CreatePlayerDto'dan türetilir ve CreatePlayerCommandHandler tarafından işlenir.
/// </summary>
public sealed class CreatePlayerCommand : IRequest<CreatePlayerResult>
{
    /// <summary>
    /// Oyuncu oluşturma verisi
    /// </summary>
    public required CreatePlayerDto PlayerData { get; set; }

    /// <summary>
    /// Komutu işleyen kullanıcı ID'si (audit için)
    /// </summary>
    public string CreatedByUserId { get; set; } = "System";

    public CreatePlayerCommand() { }

    public CreatePlayerCommand(CreatePlayerDto playerData, string createdByUserId = "System")
    {
        PlayerData = playerData;
        CreatedByUserId = createdByUserId;
    }
}

/// <summary>
/// CreatePlayerCommand'ın sonucunu temsil eder.
/// </summary>
public sealed class CreatePlayerResult
{
    /// <summary>
    /// Başarılı mı?
    /// </summary>
    public required bool IsSuccess { get; set; }

    /// <summary>
    /// Oluşturulan oyuncu ID'si
    /// </summary>
    public Guid? PlayerId { get; set; }

    /// <summary>
    /// Hata mesajı (başarısız ise)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Hata kodu
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Operasyon mesajı (ek bilgi veya uyarı)
    /// </summary>
    public string? Message { get; set; }

    public static CreatePlayerResult SuccessResult(Guid playerId, string? message = null) =>
        new()
        {
            IsSuccess = true,
            PlayerId = playerId,
            Message = message ?? "Oyuncu başarıyla oluşturuldu."
        };

    public static CreatePlayerResult FailureResult(string errorMessage, string? errorCode = null) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode ?? "CREATION_FAILED"
        };

    public static CreatePlayerResult ValidationFailureResult(string errorMessage) =>
        new()
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = "VALIDATION_ERROR"
        };
}
