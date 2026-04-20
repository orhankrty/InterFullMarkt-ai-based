using AutoMapper;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Application.DTOs;

namespace InterFullMarkt.Application.Mappings;

public class ClubProfile : Profile
{
    public ClubProfile()
    {
        CreateMap<Club, ClubDto>()
            .ForMember(dest => dest.BudgetAmount, opt => opt.MapFrom(src => src.Budget != null ? src.Budget.Amount : (decimal?)null))
            .ForMember(dest => dest.SquadCount, opt => opt.MapFrom(src => src.Players.Count))
            .ForMember(dest => dest.TotalSquadValue, opt => opt.MapFrom(src => src.CalculateTotalSquadValue() != null ? (decimal?)src.CalculateTotalSquadValue()!.Amount : null));
    }
}
