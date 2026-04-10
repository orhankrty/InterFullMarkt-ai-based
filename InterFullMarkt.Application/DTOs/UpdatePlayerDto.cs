namespace InterFullMarkt.Application.DTOs;

public class UpdatePlayerDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int Position { get; set; }
    public string NationalityCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public string? PreferredFoot { get; set; }
    public int? JerseyNumber { get; set; }
    public decimal MarketValueAmount { get; set; }
    public string Currency { get; set; } = "EUR";
    public Guid? CurrentClubId { get; set; }
    public string? ImageUrl { get; set; }
}
