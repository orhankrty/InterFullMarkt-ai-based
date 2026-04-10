namespace InterFullMarkt.Application.Features.Clubs.Queries.GetAllClubs;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public class GetAllClubsQueryHandler : IRequestHandler<GetAllClubsQuery, List<ClubDto>>
{
    private readonly IDbContext _dbContext;

    public GetAllClubsQueryHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ClubDto>> Handle(GetAllClubsQuery request, CancellationToken cancellationToken)
    {
        var clubs = await _dbContext.Clubs
            .Include(c => c.Players)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return clubs.Select(c => new ClubDto
        {
            Id = c.Id,
            Name = c.Name,
            ShortName = c.ShortName,
            FoundingYear = c.FoundingYear,
            StadiumName = c.StadiumName,
            Colors = c.Colors,
            LogoUrl = c.LogoUrl,
            Description = c.Description,
            BudgetAmount = c.Budget?.Amount,
            SquadCount = c.GetSquadCount(),
            TotalSquadValue = c.CalculateTotalSquadValue()?.Amount
        }).ToList();
    }
}