namespace InterFullMarkt.Application.Features.Leagues.Queries.GetAllLeagues;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public sealed class GetAllLeaguesQuery : IRequest<List<LeagueDto>> { }

public class LeagueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int Tier { get; set; }
}

public sealed class GetAllLeaguesQueryHandler : IRequestHandler<GetAllLeaguesQuery, List<LeagueDto>>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllLeaguesQueryHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<LeagueDto>> Handle(GetAllLeaguesQuery request, CancellationToken cancellationToken)
    {
        var leagues = await _dbContext.Leagues
            .OrderBy(l => l.Tier)
            .ThenBy(l => l.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Simple mapping without profile for speed
        return leagues.Select(l => new LeagueDto
        {
            Id = l.Id,
            Name = l.Name,
            CountryName = l.Country.CountryName,
            Tier = l.Tier
        }).ToList();
    }
}
