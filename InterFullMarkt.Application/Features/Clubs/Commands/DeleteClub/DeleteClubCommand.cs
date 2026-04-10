namespace InterFullMarkt.Application.Features.Clubs.Commands.DeleteClub;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Application.Abstractions;

public sealed record DeleteClubCommand(Guid Id, string DeletedByUserId) : IRequest<bool>;

public sealed class DeleteClubCommandHandler : IRequestHandler<DeleteClubCommand, bool>
{
    private readonly IDbContext _dbContext;

    public DeleteClubCommandHandler(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(DeleteClubCommand request, CancellationToken cancellationToken)
    {
        var club = await _dbContext.Clubs
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (club == null) return false;

        club.Delete(request.DeletedByUserId);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
