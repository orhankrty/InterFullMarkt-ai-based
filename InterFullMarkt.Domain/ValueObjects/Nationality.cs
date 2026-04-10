namespace InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Ülke ve bayrak kodu kombinasyonnu temsil eden değer nesnesi.
/// Immutable yapıdadır.
/// </summary>
public sealed class Nationality : IEquatable<Nationality>
{
    /// <summary>
    /// Ülke adı
    /// </summary>
    public string CountryName { get; }

    /// <summary>
    /// Ülke kodu (ISO 3166-1 alpha-2, örn: TR, DE, FR)
    /// </summary>
    public string CountryCode { get; }

    /// <summary>
    /// Bayrak emoji'si
    /// </summary>
    public string FlagEmoji { get; }

    private Nationality(string countryName, string countryCode, string flagEmoji)
    {
        Protect.Against.NullOrEmpty(countryName, nameof(countryName));
        Protect.Against.NullOrEmpty(countryCode, nameof(countryCode));

        if (countryCode.Length != 2)
            throw new ArgumentException("Ülke kodu 2 karakter olmalıdır (ISO 3166-1 alpha-2)", nameof(countryCode));

        CountryName = countryName;
        CountryCode = countryCode.ToUpperInvariant();
        FlagEmoji = flagEmoji ?? "🏳️";
    }

    /// <summary>
    /// Milliyeti oluşturur
    /// </summary>
    public static Nationality Create(string countryName, string countryCode, string flagEmoji = "🏳️")
        => new(countryName, countryCode, flagEmoji);

    /// <summary>
    /// Kodu baz alarak milliyeti oluşturur
    /// </summary>
    public static Nationality CreateByCode(string countryCode)
    {
        var nationality = GetByCode(countryCode);
        if (nationality is null)
            throw new ArgumentException($"'{countryCode}' kodu için milliyeti bulunamadı", nameof(countryCode));
        return nationality;
    }

    /// <summary>
    /// Bilinen ülkelerin önceden tanımlanmış listesi
    /// </summary>
    private static readonly Dictionary<string, (string Name, string Flag)> KnownCountries = new()
    {
        { "TR", ("Turkey", "🇹🇷") },
        { "DE", ("Germany", "🇩🇪") },
        { "ES", ("Spain", "🇪🇸") },
        { "IT", ("Italy", "🇮🇹") },
        { "FR", ("France", "🇫🇷") },
        { "GB", ("United Kingdom", "🇬🇧") },
        { "PT", ("Portugal", "🇵🇹") },
        { "NL", ("Netherlands", "🇳🇱") },
        { "BR", ("Brazil", "🇧🇷") },
        { "AR", ("Argentina", "🇦🇷") },
        { "FR", ("France", "🇫🇷") },
        { "BE", ("Belgium", "🇧🇪") },
        { "AT", ("Austria", "🇦🇹") },
        { "CH", ("Switzerland", "🇨🇭") },
        { "SE", ("Sweden", "🇸🇪") },
        { "NO", ("Norway", "🇳🇴") },
        { "DK", ("Denmark", "🇩🇰") },
        { "GR", ("Greece", "🇬🇷") },
        { "CZ", ("Czech Republic", "🇨🇿") },
        { "PL", ("Poland", "🇵🇱") },
        { "UY", ("Uruguay", "🇺🇾") },
        { "CO", ("Colombia", "🇨🇴") },
        { "MA", ("Morocco", "🇲🇦") },
    };

    /// <summary>
    /// Kod baz alarak ülkeyi bulur
    /// </summary>
    public static Nationality? GetByCode(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
            return null;

        var upperCode = countryCode.ToUpperInvariant();
        return KnownCountries.TryGetValue(upperCode, out var country)
            ? new Nationality(country.Name, upperCode, country.Flag)
            : null;
    }

    public bool Equals(Nationality? other)
        => other is not null && CountryCode == other.CountryCode;

    public override bool Equals(object? obj)
        => obj is Nationality other && Equals(other);

    public override int GetHashCode()
        => CountryCode.GetHashCode();

    public override string ToString()
        => $"{FlagEmoji} {CountryName} ({CountryCode})";

    public static bool operator ==(Nationality? left, Nationality? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Nationality? left, Nationality? right)
        => !(left == right);
}
