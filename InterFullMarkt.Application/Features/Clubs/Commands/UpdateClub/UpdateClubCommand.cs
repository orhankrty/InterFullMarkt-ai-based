namespace InterFullMarkt.Application.Features.Clubs.Commands.UpdateClub;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain.ValueObjects;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public sealed record UpdateClubCommand(UpdateClubDto ClubData, string UpdatedByUserId) : IRequest<bool>;

public sealed class UpdateClubCommandHandler : IRequestHandler<UpdateClubCommand, bool>
{
    private readonly IDbContext _dbContext;

    public UpdateClubCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(UpdateClubCommand request, CancellationToken cancellationToken)
    {
        var data = request.ClubData;
        var club = await _dbContext.Clubs
            .FirstOrDefaultAsync(c => c.Id == data.Id, cancellationToken);

        if (club == null) return false;

        club.Name = data.Name;
        club.ShortName = data.ShortName;
        club.FoundingYear = data.FoundingYear;
        club.StadiumName = data.StadiumName;
        club.Colors = data.Colors;
        club.LeagueId = data.LeagueId;
        club.LogoUrl = data.LogoUrl;
        club.UpdatedByUserId = request.UpdatedByUserId;

        club.UpdateBudget(Money.Create(data.BudgetAmount, data.Currency));

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
