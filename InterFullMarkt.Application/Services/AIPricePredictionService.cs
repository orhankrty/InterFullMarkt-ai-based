namespace InterFullMarkt.Application.Services;

using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// AI-based price prediction engine for players
/// Uses machine learning-inspired algorithms based on player attributes and market factors
/// </summary>
public sealed class AIPricePredictionService
{
    /// <summary>
    /// Predict player's market value 6-12 months forward
    /// </summary>
    public class PredictionResult
    {
        public decimal PredictedValue { get; set; }
        public string PredictedValueFormatted { get; set; } = string.Empty;
        public decimal ChangeAmount { get; set; }
        public decimal ChangePercentage { get; set; }
        public string Confidence { get; set; } = "Medium"; // Low, Medium, High
        public string Reasoning { get; set; } = string.Empty;
    }

    /// <summary>
    /// Predict future market value of a player
    /// Formula: CurrentValue × AgeMultiplier × PerformanceMultiplier × PositionMultiplier × LeagueCoefficient
    /// </summary>
    public PredictionResult PredictMarketValue(Player player, decimal leagueCoefficient = 1.0m)
    {
        if (player == null || player.MarketValue == null)
            return CreateDefaultPrediction(player?.MarketValue?.Amount ?? 40_000_000);

        var currentValue = player.MarketValue.Amount;
        var age = CalculateAge(player.DateOfBirth);

        // Step 1: Age-based multiplier (peak 23-28, decline after 32)
        var ageMultiplier = CalculateAgeMultiplier(age);

        // Step 2: Position-based scarcity multiplier
        var positionMultiplier = CalculatePositionMultiplier(player.Position);

        // Step 3: League coefficient (normalized: 0.7-1.2)
        var normalizedLeagueCoeff = Math.Max(0.7m, Math.Min(1.2m, leagueCoefficient / 10m));

        // Step 4: Performance/Physical condition multiplier
        var performanceMultiplier = CalculatePerformanceMultiplier(player);

        // Final prediction
        var predictedValue = currentValue 
            * ageMultiplier 
            * positionMultiplier 
            * normalizedLeagueCoeff
            * performanceMultiplier;

        var changeAmount = predictedValue - currentValue;
        var changePercentage = (changeAmount / currentValue) * 100;
        var confidence = DetermineConfidence(age, player.Position);

        // Create reasoning
        var reasoning = GenerateReasoning(age, ageMultiplier, changePercentage, confidence);

        return new PredictionResult
        {
            PredictedValue = predictedValue,
            PredictedValueFormatted = $"€{(predictedValue / 1_000_000):F1}M",
            ChangeAmount = changeAmount,
            ChangePercentage = changePercentage,
            Confidence = confidence,
            Reasoning = reasoning
        };
    }

    /// <summary>
    /// Age multiplier: young talents peak at 26-28, decline after 32
    /// </summary>
    private static decimal CalculateAgeMultiplier(int age)
    {
        return age switch
        {
            <= 19 => 1.22m, // 22% potential (very young, high upside)
            <= 22 => 1.15m, // 15% potential
            <= 25 => 1.08m, // 8% potential (approaching peak)
            <= 28 => 1.0m,  // Peak years, no change
            <= 30 => 0.95m, // -5% decline
            <= 32 => 0.88m, // -12% decline
            <= 34 => 0.75m, // -25% decline
            _ => 0.55m      // -45% decline (veteran phase)
        };
    }

    /// <summary>
    /// Position scarcity: strikers more valuable, goalkeepers less so
    /// </summary>
    private static decimal CalculatePositionMultiplier(InterFullMarkt.Domain.Enums.PlayerPosition position)
    {
        return position switch
        {
            InterFullMarkt.Domain.Enums.PlayerPosition.ST => 1.18m, // Strikers: +18% premium
            InterFullMarkt.Domain.Enums.PlayerPosition.CM => 1.12m, // Midfielders: +12%
            InterFullMarkt.Domain.Enums.PlayerPosition.CB => 1.08m, // Defenders: +8%
            InterFullMarkt.Domain.Enums.PlayerPosition.GK => 0.92m, // Goalkeepers: -8%
            _ => 1.0m
        };
    }

    /// <summary>
    /// Performance multiplier based on physical attributes
    /// Taller, heavier (muscular) players slightly more valuable
    /// </summary>
    private static decimal CalculatePerformanceMultiplier(Player player)
    {
        var mult = 1.0m;

        // Height factor (optimal 180-200cm)
        if (player.Height >= 185 && player.Height <= 195)
            mult += 0.02m;

        // Weight factor (good muscle mass)
        if (player.Weight >= 75 && player.Weight <= 95)
            mult += 0.02m;

        // Small boost for well-rounded stats
        if (player.Height > 0 && player.Weight > 0)
            mult += 0.01m;

        return mult;
    }

    /// <summary>
    /// Determine confidence level based on age and position
    /// </summary>
    private static string DetermineConfidence(int age, InterFullMarkt.Domain.Enums.PlayerPosition position)
    {
        if (age >= 23 && age <= 28) // Peak years
            return "High";
        
        if (age >= 19 && age < 23) // Rising stars
            return "High";
        
        if (age >= 29 && age <= 34) // Declining phase (less predictable)
            return "Medium";
        
        return "Low"; // Very young or very old (unpredictable)
    }

    /// <summary>
    /// Generate human-readable reasoning for the prediction
    /// </summary>
    private static string GenerateReasoning(int age, decimal ageMultiplier, decimal changePercentage, string confidence)
    {
        var reason = age switch
        {
            <= 21 => "Young talent with significant growth potential",
            <= 26 => "Prime years with stable or increasing value",
            <= 29 => "Entering decline phase, but still competitive",
            <= 32 => "Veteran phase, value decreasing",
            _ => "Late career phase, significant depreciation expected"
        };

        if (changePercentage > 5)
            reason += ". Strong upside momentum.";
        else if (changePercentage > 0)
            reason += ". Moderate growth expected.";
        else if (changePercentage >= -5)
            reason += ". Relatively stable value.";
        else
            reason += ". Depreciation likely.";

        reason += $" ({confidence} confidence)";

        return reason;
    }

    /// <summary>
    /// Calculate player age from date of birth
    /// </summary>
    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    /// <summary>
    /// Create default prediction (no player data available)
    /// </summary>
    private static PredictionResult CreateDefaultPrediction(decimal baseValue)
    {
        return new PredictionResult
        {
            PredictedValue = baseValue * 1.0m,
            PredictedValueFormatted = $"€{(baseValue / 1_000_000):F1}M",
            ChangeAmount = 0,
            ChangePercentage = 0,
            Confidence = "Low",
            Reasoning = "Insufficient data for accurate prediction"
        };
    }
}
