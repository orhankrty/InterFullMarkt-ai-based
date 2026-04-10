namespace InterFullMarkt.Tests.Domain.ValueObjects;

using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Money value object unit tests
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmountAndCurrency_ReturnsMoneyObject()
    {
        // Arrange
        var amount = 50_000_000m;
        var currency = "EUR";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        Assert.NotNull(money);
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Fact]
    public void Create_WithZeroAmount_ReturnsZeroMoney()
    {
        // Arrange & Act
        var money = Money.Create(0, "USD");

        // Assert
        Assert.Equal(0, money.Amount);
    }

    [Fact]
    public void Add_TwoMoneyObjectsWithSameCurrency_ReturnsSummedMoney()
    {
        // Arrange
        var money1 = Money.Create(30_000_000, "EUR");
        var money2 = Money.Create(20_000_000, "EUR");

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(50_000_000, result.Amount);
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public void Subtract_FromLargerMoney_ReturnsCorrectDifference()
    {
        // Arrange
        var money1 = Money.Create(100_000_000, "EUR");
        var money2 = Money.Create(30_000_000, "EUR");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(70_000_000, result.Amount);
    }

    [Fact]
    public void EqualityOperator_WithSameAmountAndCurrency_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(50_000_000, "EUR");
        var money2 = Money.Create(50_000_000, "EUR");

        // Act & Assert
        Assert.True(money1 == money2);
    }

    [Fact]
    public void GreaterThanOperator_WithLargerValue_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(100_000_000, "EUR");
        var money2 = Money.Create(50_000_000, "EUR");

        // Act & Assert
        Assert.True(money1 > money2);
    }
}
