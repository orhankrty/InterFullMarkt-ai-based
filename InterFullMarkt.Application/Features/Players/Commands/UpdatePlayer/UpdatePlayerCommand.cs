namespace InterFullMarkt.Application.Features.Players.Commands.UpdatePlayer;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.Enums;
using InterFullMarkt.Domain.ValueObjects;
using InterFullMarkt.Application.Abstractions;
using InterFullMarkt.Application.DTOs;

public sealed record UpdatePlayerCommand(UpdatePlayerDto PlayerData, string UpdatedByUserId) : IRequest<bool>;

public sealed class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, bool>
{
    private readonly IDbContext _dbContext;

    public UpdatePlayerCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
    {
        var data = request.PlayerData;
        var player = await _dbContext.Players
            .Include(p => p.CurrentClub)
            .FirstOrDefaultAsync(p => p.Id == data.Id, cancellationToken);

        if (player == null) return false;

        player.FullName = data.FullName;
        player.Position = (PlayerPosition)data.Position;
        player.Nationality = Nationality.CreateByCode(data.NationalityCode) ?? player.Nationality;
        player.DateOfBirth = data.DateOfBirth;
        player.Height = data.Height;
        player.Weight = data.Weight;
        player.PreferredFoot = data.PreferredFoot;
        player.JerseyNumber = data.JerseyNumber;
        player.ImageUrl = data.ImageUrl;
        player.UpdatedByUserId = request.UpdatedByUserId;

        player.UpdateMarketValue(Money.Create(data.MarketValueAmount, data.Currency));

        if (data.CurrentClubId != player.CurrentClubId)
        {
            if (data.CurrentClubId.HasValue)
                player.TransferToClub(data.CurrentClubId.Value);
            else
                player.RemoveFromClub();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
