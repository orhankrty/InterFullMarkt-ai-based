namespace InterFullMarkt.Infrastructure.Data.Seeds;

using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedGalatasarayAsync(InterFullMarktDbContext context)
    {
        // Eğer veritabanında Galatasaray zaten varsa, verileri tekrar ekleme (Dublicate önlemi)
        if (await context.Clubs.AnyAsync(c => c.Name == "Galatasaray"))
            return;

        // Sistemdeki mevcut bir ligi bul, yoksa rastgele bir lig ID'si kullan
        var league = await context.Leagues.FirstOrDefaultAsync();
        var leagueId = league?.Id ?? Guid.NewGuid();

        // 1. Kulüp Oluşturma (Logo ve Temel Bilgiler)
        var galatasaray = new Club(
            name: "Galatasaray",
            shortName: "GS",
            foundingYear: 1905,
            stadiumName: "RAMS Park",
            colors: "Sarı, Kırmızı",
            leagueId: leagueId,
            initialBudget: Money.Create(50000000, "EUR")
        )
        {
            LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f6/Galatasaray_Sports_Club_Logo.png/500px-Galatasaray_Sports_Club_Logo.png",
            Description = "Türkiye'nin Avrupa'da kupa kazanan ilk ve en başarılı futbol kulübü.",
            CreatedByUserId = "System"
        };

        context.Clubs.Add(galatasaray);

        // 2. Oyuncu Kadrosunu Oluşturma (Resim url'leri UI Avatar servisi ile otomatik ad-soyad baş harflerinden üretilebilir)
        var players = new List<Player>
        {
            CreatePlayer("Fernando Muslera", PlayerPosition.GK, "UY", new DateTime(1986, 6, 16), 190, 84, "Right", 1, 1500000, galatasaray.Id),
            CreatePlayer("Mauro Icardi", PlayerPosition.ST, "AR", new DateTime(1993, 2, 19), 181, 75, "Right", 9, 18000000, galatasaray.Id),
            CreatePlayer("Dries Mertens", PlayerPosition.CM, "BE", new DateTime(1987, 5, 6), 169, 61, "Right", 10, 2000000, galatasaray.Id),
            CreatePlayer("Barış Alper Yılmaz", PlayerPosition.ST, "TR", new DateTime(2000, 5, 23), 186, 80, "Right", 53, 20000000, galatasaray.Id),
            CreatePlayer("Lucas Torreira", PlayerPosition.CM, "UY", new DateTime(1996, 2, 11), 166, 65, "Right", 34, 15000000, galatasaray.Id),
            CreatePlayer("Davinson Sánchez", PlayerPosition.CB, "CO", new DateTime(1996, 6, 12), 187, 79, "Right", 25, 17000000, galatasaray.Id),
            CreatePlayer("Abdülkerim Bardakcı", PlayerPosition.CB, "TR", new DateTime(1994, 9, 7), 185, 80, "Left", 42, 9000000, galatasaray.Id),
            CreatePlayer("Hakim Ziyech", PlayerPosition.CM, "MA", new DateTime(1993, 3, 19), 181, 65, "Left", 22, 9000000, galatasaray.Id)
        };

        context.Players.AddRange(players);
        await context.SaveChangesAsync();
    }

    private static Player CreatePlayer(string name, PlayerPosition pos, string country, DateTime dob, int height, int weight, string foot, int number, decimal value, Guid clubId)
    {
        var player = new Player(name, pos, Nationality.CreateByCode(country), dob, height, weight)
        {
            PreferredFoot = foot, JerseyNumber = number, CurrentClubId = clubId, CreatedByUserId = "System"
            // Eğer 'Player' modelinizde ImageUrl adında bir alanınız varsa alttaki satırın yorumunu kaldırabilirsiniz:
            // ImageUrl = $"https://ui-avatars.com/api/?name={name.Replace(" ", "+")}&background=random&size=200"
        };
        player.UpdateMarketValue(Money.Create(value, "EUR"));
        return player;
    }
}