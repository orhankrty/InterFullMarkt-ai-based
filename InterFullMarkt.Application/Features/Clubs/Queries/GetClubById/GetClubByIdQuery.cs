namespace InterFullMarkt.Application.Features.Clubs.Queries.GetClubById;

using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public sealed record GetClubByIdQuery(Guid Id) : IRequest<ClubDto?>;

public sealed class GetClubByIdQueryHandler : IRequestHandler<GetClubByIdQuery, ClubDto?>
{
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetClubByIdQueryHandler(IDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClubDto?> Handle(GetClubByIdQuery request, CancellationToken cancellationToken)
    {
        var club = await _dbContext.Clubs
            .Include(c => c.Players)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return club != null ? _mapper.Map<ClubDto>(club) : null;
    }
}
