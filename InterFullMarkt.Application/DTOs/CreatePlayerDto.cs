namespace InterFullMarkt.Application.DTOs;

/// <summary>
/// Yeni oyuncu oluştururken kullanılan DTO.
/// Client'tan gelen verileri taşır, FluentValidation tarafından doğrulanır.
/// </summary>
public sealed class CreatePlayerDto
{
    /// <summary>
    /// Oyuncu adı ve soyadı (zorunlu)
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// Pozisyon enum değeri (1=GK, 2=CB, 3=CM, 4=ST)
    /// </summary>
    public required int Position { get; set; }

    /// <summary>
    /// Ülke kodu (ISO 3166-1 alpha-2, örn: TR, DE, FR)
    /// </summary>
    public required string NationalityCode { get; set; }

    /// <summary>
    /// Doğum tarihi
    /// </summary>
    public required DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Boy (cm, zorunlu, minimum 150)
    /// </summary>
    public required int Height { get; set; }

    /// <summary>
    /// Kilo (kg, zorunlu, minimum 40)
    /// </summary>
    public required decimal Weight { get; set; }

    /// <summary>
    /// Tercih edilen ayak (sol/sağ, optional, default: "Right")
    /// </summary>
    public string PreferredFoot { get; set; } = "Right";

    /// <summary>
    /// Başlangıç piyasa değeri (optional)
    /// </summary>
    public decimal? InitialMarketValue { get; set; }

    /// <summary>
    /// Para birimi (optional, default: EUR)
    /// </summary>
    public string Currency { get; set; } = "EUR";

    /// <summary>
    /// Forma numarası (optional, 1-99 arası)
    /// </summary>
    public int? JerseyNumber { get; set; }

    /// <summary>
    /// Kulüp ID'si (optional - başta kulüpsüz eklenebilir)
    /// </summary>
    public Guid? CurrentClubId { get; set; }
}
