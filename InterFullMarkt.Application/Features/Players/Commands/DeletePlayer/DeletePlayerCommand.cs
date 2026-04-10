namespace InterFullMarkt.Application.Features.Players.Commands.DeletePlayer;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;

public sealed record DeletePlayerCommand(Guid Id, string DeletedByUserId) : IRequest<bool>;

public sealed class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, bool>
{
    private readonly IDbContext _dbContext;

    public DeletePlayerCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
    {
        var player = await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (player == null) return false;

        player.Delete(request.DeletedByUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
