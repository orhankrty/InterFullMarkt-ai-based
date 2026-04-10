namespace InterFullMarkt.Application.Features.Dashboard.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;

/// <summary>
/// Handler for GetDashboardQuery
/// Aggregates dashboard widgets: top players, value gainers, league statistics
/// </summary>
public sealed class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, GetDashboardResult>
{
    private readonly IDbContext _dbContext;

    public GetDashboardQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetDashboardResult> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        // Top 5 richest players
        var topPlayers = await _dbContext.Players
            .Include(p => p.CurrentClub)
            .AsNoTracking()
            .Where(p => p.MarketValue != null)
            .OrderByDescending(p => p.MarketValue!.Amount)
            .Take(5)
            .Select(p => new TopPlayerWidget
            {
                PlayerId = p.Id,
                FullName = p.FullName ?? "Bilinmiyor",
                Position = p.Position.ToString(),
                Nationality = p.Nationality != null ? p.Nationality.CountryName : "Bilinmiyor",
                NationalityFlag = p.Nationality != null ? p.Nationality.FlagEmoji : "🏳️",
                ClubName = p.CurrentClub != null ? p.CurrentClub.Name : "Kulüpsüz",
                MarketValue = p.MarketValue != null ? $"€{(p.MarketValue.Amount / 1_000_000):F1}M" : "N/A",
                MarketValueAmount = p.MarketValue != null ? p.MarketValue.Amount : 0
            })
            .ToListAsync(cancellationToken);

        // Value gainers (simulated: players with best potential based on age and position)
        var allPlayers = await _dbContext.Players
            .Include(p => p.CurrentClub)
            .AsNoTracking()
            .Where(p => p.MarketValue != null)
            .ToListAsync(cancellationToken);

        var valueGainers = allPlayers
            .Select(p => new
            {
                Player = p,
                PotentialGain = CalculatePotentialGain(p)
            })
            .OrderByDescending(x => x.PotentialGain.PercentageIncrease)
            .Take(5)
            .Select(x => new ValueGainerWidget
            {
                PlayerId = x.Player.Id,
                FullName = x.Player.FullName ?? "Bilinmiyor",
                Position = x.Player.Position.ToString(),
                ClubName = x.Player.CurrentClub?.Name ?? "Kulüpsüz",
                ValueIncrease = x.PotentialGain.AbsoluteIncrease >= 0 
                    ? $"+€{(x.PotentialGain.AbsoluteIncrease / 1_000_000):F1}M"
                    : $"-€{(Math.Abs(x.PotentialGain.AbsoluteIncrease) / 1_000_000):F1}M",
                PercentageGain = x.PotentialGain.PercentageIncrease
            })
            .ToList();

        // League statistics
        var leagues = await _dbContext.Leagues
            .Include(l => l.Clubs)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var leagueStats = new List<LeagueStatWidget>();

        foreach (var league in leagues)
        {
            var leaguePlayers = await _dbContext.Players
                .AsNoTracking()
                .Where(p => p.CurrentClubId != null && 
                            league.Clubs.Select(c => c.Id).Contains(p.CurrentClubId.Value))
                .ToListAsync(cancellationToken);

            var totalSquadValue = leaguePlayers
                .Where(p => p.MarketValue != null)
                .Sum(p => p.MarketValue!.Amount);

            var averagePlayerValue = leaguePlayers.Any(p => p.MarketValue != null)
                ? leaguePlayers.Where(p => p.MarketValue != null).Average(p => p.MarketValue!.Amount)
                : 0m;

            leagueStats.Add(new LeagueStatWidget
            {
                LeagueName = league.Name,
                Country = league.Country?.CountryName ?? "Bilinmiyor",
                CountryFlag = league.Country?.FlagEmoji ?? "🏳️",
                TotalClubs = league.Clubs.Count,
                TotalPlayers = leaguePlayers.Count,
                AveragePlayerValue = $"€{(averagePlayerValue / 1_000_000):F1}M",
                TotalSquadValue = $"€{(totalSquadValue / 1_000_000_000):F2}B",
                TotalSquadValueAmount = totalSquadValue
            });
        }

        // Global stats
        var totalPlayers = await _dbContext.Players.CountAsync(cancellationToken);
        var totalClubs = await _dbContext.Clubs.CountAsync(cancellationToken);
        var totalMarketValue = allPlayers
            .Where(p => p.MarketValue != null)
            .Sum(p => p.MarketValue!.Amount);

        return new GetDashboardResult
        {
            TopPlayers = topPlayers,
            ValueGainers = valueGainers,
            LeagueStats = leagueStats.OrderByDescending(l => l.TotalSquadValueAmount).ToList(),
            TotalPlayers = totalPlayers,
            TotalClubs = totalClubs,
            TotalMarketValue = $"€{(totalMarketValue / 1_000_000_000):F2}B"
        };
    }

    /// <summary>
    /// Calculate potential value gain based on age and position
    /// </summary>
    private static (decimal AbsoluteIncrease, decimal PercentageIncrease) CalculatePotentialGain(
        InterFullMarkt.Domain.Entities.Player player)
    {
        var age = DateTime.Now.Year - player.DateOfBirth.Year;
        if (player.DateOfBirth.Date > DateTime.Now.AddYears(-age)) age--;

        if (player.MarketValue == null || player.MarketValue.Amount == 0)
            return (0, 0);

        // Young talents (18-23) have high growth potential
        var ageMultiplier = age switch
        {
            <= 21 => 0.18m, // 18% potential growth
            <= 23 => 0.15m,
            <= 26 => 0.08m,
            <= 29 => 0.02m,
            _ => -0.05m // Decline
        };

        var absoluteGain = player.MarketValue.Amount * ageMultiplier;
        var percentageGain = (ageMultiplier * 100);

        return (absoluteGain, percentageGain);
    }
}
