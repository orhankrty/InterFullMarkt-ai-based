namespace InterFullMarkt.Tests.Application.Services;

using InterFullMarkt.Application.Services;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Unit tests for AIPricePredictionService
/// </summary>
public class AIPricePredictionServiceTests
{
    private readonly AIPricePredictionService _service;

    public AIPricePredictionServiceTests()
    {
        _service = new AIPricePredictionService();
    }

    [Fact]
    public void PredictMarketValue_ForYoungTalent_PredictsPriceIncrease()
    {
        // Arrange
        var player = CreateTestPlayer(
            age: 21,
            position: PlayerPosition.CM,
            currentValue: 40_000_000
        );

        // Act
        var prediction = _service.PredictMarketValue(player, leagueCoefficient: 8.5m);

        // Assert
        Assert.NotNull(prediction);
        Assert.True(prediction.ChangePercentage > 0, "Young talent should have positive growth");
        Assert.Contains("Growth potential", prediction.Reasoning);
    }

    [Fact]
    public void PredictMarketValue_ForPeakAgePlayer_PredictStableValue()
    {
        // Arrange
        var player = CreateTestPlayer(
            age: 26,
            position: PlayerPosition.ST,
            currentValue: 100_000_000
        );

        // Act
        var prediction = _service.PredictMarketValue(player, leagueCoefficient: 9.0m);

        // Assert
        Assert.NotNull(prediction);
        Assert.Contains("Prime years", prediction.Reasoning);
        Assert.Equal("High", prediction.Confidence);
    }

    [Fact]
    public void PredictMarketValue_ForVeteranPlayer_PredictsPriceDecrease()
    {
        // Arrange
        var player = CreateTestPlayer(
            age: 34,
            position: PlayerPosition.CB,
            currentValue: 30_000_000
        );

        // Act
        var prediction = _service.PredictMarketValue(player, leagueCoefficient: 7.0m);

        // Assert
        Assert.NotNull(prediction);
        Assert.True(prediction.ChangePercentage < 0, "Veteran player should have negative growth");
        Assert.Contains("Late career", prediction.Reasoning);
    }

    [Fact]
    public void PredictMarketValue_StrikerPositionGetsBonus_ComparedToGoalkeeper()
    {
        // Arrange
        var strikerPlayer = CreateTestPlayer(age: 25, position: PlayerPosition.ST, currentValue: 50_000_000);
        var goalkeeperPlayer = CreateTestPlayer(age: 25, position: PlayerPosition.GK, currentValue: 50_000_000);

        // Act
        var strikerPrediction = _service.PredictMarketValue(strikerPlayer);
        var goalkeeperPrediction = _service.PredictMarketValue(goalkeeperPlayer);

        // Assert
        Assert.True(strikerPrediction.PredictedValue > goalkeeperPrediction.PredictedValue,
            "Strikers should be predicted higher than goalkeepers");
    }

    [Fact]
    public void PredictMarketValue_WithNullMarketValue_ReturnsDefaultPrediction()
    {
        // Arrange
        var player = new Player(
            fullName: "Test Player",
            position: PlayerPosition.CM,
            nationality: Nationality.CreateByCode("TR"),
            dateOfBirth: DateTime.Now.AddYears(-25),
            height: 185,
            weight: 80,
            preferredFoot: "Right"
        );
        player.MarketValue = null;

        // Act
        var prediction = _service.PredictMarketValue(player);

        // Assert
        Assert.NotNull(prediction);
        Assert.Equal("Low", prediction.Confidence);
        Assert.Contains("Insufficient data", prediction.Reasoning);
    }

    // Helper method to create test player
    private Player CreateTestPlayer(int age, PlayerPosition position, decimal currentValue)
    {
        var dateOfBirth = DateTime.Now.AddYears(-age);
        var player = new Player(
            fullName: $"Test Player {position}",
            position: position,
            nationality: Nationality.CreateByCode("ES"),
            dateOfBirth: dateOfBirth,
            height: 185,
            weight: 82,
            preferredFoot: "Right"
        );

        player.MarketValue = Money.Create(currentValue, "EUR");
        return player;
    }
}
