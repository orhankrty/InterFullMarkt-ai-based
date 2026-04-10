namespace InterFullMarkt.Application.Features.Dashboard.Queries;

/// <summary>
/// Dashboard result with all widgets
/// </summary>
public sealed class GetDashboardResult
{
    public List<TopPlayerWidget> TopPlayers { get; set; } = new();
    public List<ValueGainerWidget> ValueGainers { get; set; } = new();
    public List<LeagueStatWidget> LeagueStats { get; set; } = new();
    public int TotalPlayers { get; set; }
    public int TotalClubs { get; set; }
    public string TotalMarketValue { get; set; } = string.Empty;
}

/// <summary>
/// Top 5 richest players
/// </summary>
public sealed class TopPlayerWidget
{
    public Guid PlayerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string NationalityFlag { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string MarketValue { get; set; } = string.Empty;
    public decimal MarketValueAmount { get; set; }
}

/// <summary>
/// Players with highest hypothetical value gains (simulated)
/// </summary>
public sealed class ValueGainerWidget
{
    public Guid PlayerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string ClubName { get; set; } = string.Empty;
    public string ValueIncrease { get; set; } = string.Empty; // e.g., "+€5M"
    public decimal PercentageGain { get; set; } // e.g., 12.5%
}

/// <summary>
/// League statistics
/// </summary>
public sealed class LeagueStatWidget
{
    public string LeagueName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryFlag { get; set; } = string.Empty;
    public int TotalClubs { get; set; }
    public int TotalPlayers { get; set; }
    public string AveragePlayerValue { get; set; } = string.Empty;
    public string TotalSquadValue { get; set; } = string.Empty;
    public decimal TotalSquadValueAmount { get; set; }
}
