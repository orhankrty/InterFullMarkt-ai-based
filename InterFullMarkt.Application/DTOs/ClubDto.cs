namespace InterFullMarkt.Application.DTOs;

public class ClubDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int FoundingYear { get; set; }
    public string StadiumName { get; set; } = string.Empty;
    public string Colors { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public decimal? BudgetAmount { get; set; }
    public int SquadCount { get; set; }
    public decimal? TotalSquadValue { get; set; }
}