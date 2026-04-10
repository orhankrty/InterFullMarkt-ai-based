namespace InterFullMarkt.Application.DTOs;

public class CreateClubDto
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int FoundingYear { get; set; }
    public string StadiumName { get; set; } = string.Empty;
    public string Colors { get; set; } = string.Empty;
    public Guid LeagueId { get; set; }
    public string? LogoUrl { get; set; }
    public decimal InitialBudget { get; set; }
    public string Currency { get; set; } = "EUR";
}
