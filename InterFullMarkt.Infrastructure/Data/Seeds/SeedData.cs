namespace InterFullMarkt.Infrastructure.Data.Seeds;

using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;

/// <summary>
/// Veritabanı başlangıç verileri (Seed Data).
/// İlk çalıştırılışta Lig, Kulüpler ve Oyuncuları yükler.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Başlangıç verilerini yapılandırır
    /// </summary>
    public static void ConfigureSeedData(ModelBuilder modelBuilder)
    {
        // Ligler
        var laLiga = CreateLaLiga();
        var premierLeague = CreatePremierLeague();

        modelBuilder.Entity<League>().HasData(laLiga, premierLeague);

        // Kulüpler
        var realMadrid = CreateRealMadrid(laLiga.Id);
        var manCity = CreateManCity(premierLeague.Id);

        modelBuilder.Entity<Club>().HasData(realMadrid, manCity);

        // Oyuncular
        var ardaGuler = CreateArdaGuler(realMadrid.Id);
        var haaland = CreateHaaland(manCity.Id);

        modelBuilder.Entity<Player>().HasData(ardaGuler, haaland);
    }

    /// <summary>
    /// La Liga (İspanyol Ligi) oluşturur
    /// </summary>
    private static League CreateLaLiga()
    {
        var id = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var league = new League(
            name: "La Liga",
            country: Nationality.CreateByCode("ES")!,
            tier: 1,
            coefficient: 8.5m)
        {
            Id = id,
            Description = "İspanya'nın en yüksek ligasyonu, Real Madrid ve Barcelona gibi kulüplerle ünlü.",
            LogoUrl = "https://www.laliga.com/logo.png",
            CreatedDate = DateTime.UtcNow.AddYears(-130),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return league;
    }

    /// <summary>
    /// Premier League (İngiliz Ligi) oluşturur
    /// </summary>
    private static League CreatePremierLeague()
    {
        var id = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var league = new League(
            name: "Premier League",
            country: Nationality.CreateByCode("GB")!,
            tier: 1,
            coefficient: 9.0m)
        {
            Id = id,
            Description = "İngiltere'nin en yüksek ligasyonu, dünya futbolunun en zengin ve rekabetçi ligası.",
            LogoUrl = "https://www.premierleague.com/logo.png",
            CreatedDate = DateTime.UtcNow.AddYears(-130),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return league;
    }

    /// <summary>
    /// Real Madrid kulübü oluşturur
    /// </summary>
    private static Club CreateRealMadrid(Guid leagueId)
    {
        var id = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var club = new Club(
            name: "Real Madrid Club de Fútbol",
            shortName: "RMA",
            foundingYear: 1902,
            stadiumName: "Santiago Bernabéu",
            colors: "White,Gold",
            leagueId: leagueId,
            initialBudget: Money.Create(800_000_000, "EUR"))
        {
            Id = id,
            Description = "İspanya'nın en başarılı futbol kulübü, 14 UEFA Şampiyonlar Ligi şampiyonluğu.",
            LogoUrl = "https://www.realmadrid.com/logo.png",
            CreatedDate = DateTime.UtcNow.AddYears(-122),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return club;
    }

    /// <summary>
    /// Manchester City kulübü oluşturur
    /// </summary>
    private static Club CreateManCity(Guid leagueId)
    {
        var id = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var club = new Club(
            name: "Manchester City Football Club",
            shortName: "MCI",
            foundingYear: 1880,
            stadiumName: "Etihad Stadium",
            colors: "Sky Blue,White",
            leagueId: leagueId,
            initialBudget: Money.Create(900_000_000, "EUR"))
        {
            Id = id,
            Description = "İngiltere'nin en başarılı takımlarından biri, Pep Guardiola yönetiminde çoğu şampiyonluk kazandı.",
            LogoUrl = "https://www.mancity.com/logo.png",
            CreatedDate = DateTime.UtcNow.AddYears(-144),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return club;
    }

    /// <summary>
    /// Arda Güler oyuncusu oluşturur
    /// </summary>
    private static Player CreateArdaGuler(Guid realMadridId)
    {
        var id = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var player = new Player(
            fullName: "Arda Güler",
            position: PlayerPosition.CM,
            nationality: Nationality.CreateByCode("TR")!,
            dateOfBirth: new DateTime(2005, 02, 24),
            height: 178,
            weight: 72)
        {
            Id = id,
            CurrentClubId = realMadridId,
            JerseyNumber = 42,
            MarketValue = Money.Create(40_000_000, "EUR"),
            CreatedDate = DateTime.UtcNow.AddMonths(-24),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return player;
    }

    /// <summary>
    /// Erling Haaland oyuncusu oluşturur
    /// </summary>
    private static Player CreateHaaland(Guid manCityId)
    {
        var id = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var player = new Player(
            fullName: "Erling Braut Haaland",
            position: PlayerPosition.ST,
            nationality: Nationality.CreateByCode("NO")!,
            dateOfBirth: new DateTime(2000, 07, 21),
            height: 194,
            weight: 88)
        {
            Id = id,
            CurrentClubId = manCityId,
            JerseyNumber = 9,
            MarketValue = Money.Create(180_000_000, "EUR"),
            CreatedDate = DateTime.UtcNow.AddMonths(-36),
            UpdatedDate = DateTime.UtcNow,
            CreatedByUserId = "System"
        };

        return player;
    }
}
