namespace InterFullMarkt.Infrastructure.Data.Seeds;

using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedGalatasarayAsync(InterFullMarktDbContext context)
    {
        // Eğer veritabanında Galatasaray zaten varsa, verileri tekrar ekleme
        if (await context.Clubs.AnyAsync(c => c.Name == "Galatasaray"))
            return;

        // 1. Önce Süper Lig'i oluştur (FK bağımlılığı için gerekli)
        var superLig = await context.Leagues.FirstOrDefaultAsync(l => l.Name == "Süper Lig");
        if (superLig == null)
        {
            superLig = new League(
                name: "Süper Lig",
                country: Nationality.CreateByCode("TR"),
                tier: 1,
                coefficient: 5.8m
            )
            {
                Description = "Türkiye'nin en üst düzey futbol ligi, 1959'dan beri düzenlenmektedir.",
                LogoUrl = "https://upload.wikimedia.org/wikipedia/tr/thumb/3/34/Trendyol_S%C3%BCper_Lig_logo.svg/200px-Trendyol_S%C3%BCper_Lig_logo.svg.png",
                CreatedByUserId = "System"
            };
            context.Leagues.Add(superLig);
            await context.SaveChangesAsync();
        }

        // 2. Galatasaray Kulübünü Oluştur
        var galatasaray = new Club(
            name: "Galatasaray",
            shortName: "GS",
            foundingYear: 1905,
            stadiumName: "RAMS Park",
            colors: "Sarı, Kırmızı",
            leagueId: superLig.Id,
            initialBudget: Money.Create(50000000, "EUR")
        )
        {
            LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f6/Galatasaray_Sports_Club_Logo.png/500px-Galatasaray_Sports_Club_Logo.png",
            Description = "Türkiye'nin Avrupa'da kupa kazanan ilk ve en başarılı futbol kulübü. 2000 yılında UEFA Kupası ve Süper Kupa'yı kazanmıştır.",
            CreatedByUserId = "System"
        };

        context.Clubs.Add(galatasaray);
        await context.SaveChangesAsync();

        // 3. Galatasaray Kadrosunu Oluştur (2024-25 Sezonu)
        var players = new List<Player>
        {
            // === KALECİLER ===
            CreatePlayer("Fernando Muslera",     PlayerPosition.GK, "UY", new DateTime(1986, 6, 16), 190, 84, "Right", 1,  1500000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/59068-1701275609.jpg"),
            
            // === DEFANS ===
            CreatePlayer("Davinson Sánchez",     PlayerPosition.CB, "CO", new DateTime(1996, 6, 12), 187, 79, "Right", 25, 17000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/344755-1695374005.jpg"),
            CreatePlayer("Abdülkerim Bardakcı",  PlayerPosition.CB, "TR", new DateTime(1994, 9, 7),  185, 80, "Left",  42, 9000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/270498-1694590799.jpg"),
            CreatePlayer("Kaan Ayhan",           PlayerPosition.CB, "TR", new DateTime(1994, 11, 10),184, 82, "Right", 4,  5000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/103498-1694590653.jpg"),
            CreatePlayer("Victor Nelsson",       PlayerPosition.CB, "DK", new DateTime(1998, 10, 14),186, 77, "Right", 3,  8000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/423494-1694590928.jpg"),

            // === ORTA SAHA ===
            CreatePlayer("Lucas Torreira",       PlayerPosition.CM, "UY", new DateTime(1996, 2, 11), 166, 65, "Right", 34, 15000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/318077-1694590876.jpg"),
            CreatePlayer("Hakim Ziyech",         PlayerPosition.CM, "MA", new DateTime(1993, 3, 19), 181, 65, "Left",  22, 9000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/217111-1694590834.jpg"),
            CreatePlayer("Dries Mertens",        PlayerPosition.CM, "BE", new DateTime(1987, 5, 6),  169, 61, "Right", 10, 2000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/58968-1694590736.jpg"),
            CreatePlayer("Kerem Demirbay",       PlayerPosition.CM, "TR", new DateTime(1993, 7, 3),  182, 75, "Left",  21, 3000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/130429-1694590760.jpg"),
            CreatePlayer("Sergio Oliveira",      PlayerPosition.CM, "PT", new DateTime(1992, 6, 2),  178, 73, "Right", 27, 5000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/128352-1694590805.jpg"),

            // === FORVET ===
            CreatePlayer("Mauro Icardi",         PlayerPosition.ST, "AR", new DateTime(1993, 2, 19), 181, 75, "Right", 9,  18000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/68175-1694590692.jpg"),
            CreatePlayer("Barış Alper Yılmaz",   PlayerPosition.ST, "TR", new DateTime(2000, 5, 23), 176, 70, "Right", 53, 20000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/667498-1694590974.jpg"),
            CreatePlayer("Kerem Aktürkoğlu",     PlayerPosition.ST, "TR", new DateTime(1998, 10, 21),178, 72, "Right", 7,  18000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/496270-1694590945.jpg"),
            CreatePlayer("Wilfried Zaha",        PlayerPosition.ST, "GB", new DateTime(1992, 11, 10),180, 72, "Right", 11, 9000000, galatasaray.Id,
                "https://img.a.transfermarkt.technology/portrait/header/145988-1694590626.jpg"),
        };

        context.Players.AddRange(players);
        await context.SaveChangesAsync();
    }

    private static Player CreatePlayer(
        string name, PlayerPosition pos, string country, DateTime dob,
        int height, int weight, string foot, int number, decimal value, Guid clubId,
        string? imageUrl = null)
    {
        var player = new Player(name, pos, Nationality.CreateByCode(country), dob, height, weight)
        {
            PreferredFoot = foot,
            JerseyNumber = number,
            CurrentClubId = clubId,
            CreatedByUserId = "System"
        };
        player.UpdateMarketValue(Money.Create(value, "EUR"));
        return player;
    }
}