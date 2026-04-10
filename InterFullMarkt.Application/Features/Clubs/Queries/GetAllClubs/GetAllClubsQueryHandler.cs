namespace InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public sealed class GetAllClubsQueryHandler : IRequestHandler<GetAllClubsQuery, List<ClubDto>>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetAllClubsQueryHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ClubDto>> Handle(GetAllClubsQuery request, CancellationToken cancellationToken)
    {
        var clubs = await _dbContext.Clubs
            .Include(c => c.Players)
            .OrderBy(c => c.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ClubDto>>(clubs);
    }
}
