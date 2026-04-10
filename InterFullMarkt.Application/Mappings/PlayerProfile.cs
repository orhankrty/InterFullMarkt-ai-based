namespace InterFullMarkt.Application.Mappings;

using AutoMapper;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Application.DTOs;

/// <summary>
/// AutoMapper profili - Domain Entity'leri ile DTO'lar arasındaki dönüşümleri tanımlar.
/// </summary>
public sealed class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        // Player Entity → PlayerDto
        CreateMap<Player, PlayerDto>()
            .ForMember(dest => dest.Position,
                opt => opt.MapFrom(src => src.Position.ToString()))
            .ForMember(dest => dest.Nationality,
                opt => opt.MapFrom(src => $"{src.Nationality.FlagEmoji} {src.Nationality.CountryName}"))
            .ForMember(dest => dest.Age,
                opt => opt.MapFrom(src => src.GetAge()))
            .ForMember(dest => dest.MarketValue,
                opt => opt.MapFrom(src => src.MarketValue != null ? $"{src.MarketValue.Amount} {src.MarketValue.Currency}" : null))
            .ForMember(dest => dest.CurrentClubName,
                opt => opt.MapFrom(src => src.CurrentClub != null ? src.CurrentClub.Name : null));

        // CreatePlayerDto → Player Entity
        // Not: Bu mapping'in Create Handler'da kullanılır
        // DTO'dan direct Entity oluşturulmaz, çünkü Entity'nin kendi kurucusu vardır
    }
}
