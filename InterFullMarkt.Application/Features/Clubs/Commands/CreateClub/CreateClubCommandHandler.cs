namespace InterFullMarkt.Application.Features.Clubs.Commands.CreateClub;

using MediatR;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.ValueObjects;
using InterFullMarkt.Application.Abstractions;

public sealed class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, Guid>
{
    private readonly IDbContext _dbContext;

    public CreateClubCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateClubCommand request, CancellationToken cancellationToken)
    {
        var data = request.ClubData;
        
        var club = new Club(
            name: data.Name,
            shortName: data.ShortName,
            foundingYear: data.FoundingYear,
            stadiumName: data.StadiumName,
            colors: data.Colors,
            leagueId: data.LeagueId,
            initialBudget: Money.Create(data.InitialBudget, data.Currency)
        )
        {
            LogoUrl = data.LogoUrl,
            CreatedByUserId = request.CreatedByUserId
        };

        _dbContext.Clubs.Add(club);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return club.Id;
    }
}
