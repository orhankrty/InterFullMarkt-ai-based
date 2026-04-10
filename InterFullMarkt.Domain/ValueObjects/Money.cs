using System.ComponentModel.DataAnnotations.Schema;

namespace InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Paranın temsili için değer nesnesi (Value Object).
/// Para birimi ve miktar bilgisini birlikte tutar.
/// Immutable yapıdadır - bir kez oluşturulduktan sonra değiştirilmez.
/// </summary>
[ComplexType]
public sealed class Money : IEquatable<Money>
{
    /// <summary>
    /// Para miktarı
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Para birimi (EUR, USD, GBP, vb.)
    /// </summary>
    public string Currency { get; private set; }

    // EF CORE İÇİN EKLENMESİ GEREKEN KISIM:
    private Money() { Currency = null!; }

    private Money(decimal amount, string currency)
    {
        Protect.Against.Negative(amount, nameof(amount));
        Protect.Against.NullOrEmpty(currency, nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Money nesnesi oluşturur
    /// </summary>
    /// <param name="amount">Para miktarı (pozitif olmalı)</param>
    /// <param name="currency">Para birimi (örn: EUR, USD)</param>
    /// <returns>Money değer nesnesi</returns>
    /// <exception cref="ArgumentException">Miktar negatif veya sıfır ise</exception>
    public static Money Create(decimal amount, string currency = "EUR")
        => new(amount, currency);

    /// <summary>
    /// Sıfır para miktarını oluşturur
    /// </summary>
    public static Money Zero(string currency = "EUR")
        => new(0, currency);

    /// <summary>
    /// İki para nesnesini toplar (aynı para biriminde olması gerekir)
    /// </summary>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Para birimleri eşleşmiyor: {Currency} vs {other.Currency}");

        return new Money(Amount + other.Amount, Currency);
    }

    /// <summary>
    /// İki para nesnesini çıkarır (aynı para biriminde olması gerekir)
    /// </summary>
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Para birimleri eşleşmiyor: {Currency} vs {other.Currency}");

        var result = Amount - other.Amount;
        return new Money(result >= 0 ? result : 0, Currency);
    }

    /// <summary>
    /// Para miktarını belirli bir yüzde ile çarpar
    /// </summary>
    public Money MultiplyByPercentage(decimal percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Yüzde 0 ile 100 arasında olmalıdır", nameof(percentage));

        return new Money(Amount * (percentage / 100), Currency);
    }

    /// <summary>
    /// Para miktarını belirli bir çarpanla çarpar
    /// </summary>
    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Çarpan negatif olamaz", nameof(multiplier));

        return new Money(Amount * multiplier, Currency);
    }

    public bool Equals(Money? other)
        => other is not null
            && Amount == other.Amount
            && Currency == other.Currency;

    public override bool Equals(object? obj)
        => obj is Money other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public override string ToString()
        => $"{Amount:F2} {Currency}";

    public static bool operator ==(Money left, Money right)
        => left.Equals(right);

    public static bool operator !=(Money left, Money right)
        => !left.Equals(right);

    public static bool operator >(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Para birimleri eşleşmiyor");
        return left.Amount > right.Amount;
    }

    public static bool operator <(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Para birimleri eşleşmiyor");
        return left.Amount < right.Amount;
    }

    public static bool operator >=(Money left, Money right)
        => left > right || left == right;

    public static bool operator <=(Money left, Money right)
        => left < right || left == right;
}
