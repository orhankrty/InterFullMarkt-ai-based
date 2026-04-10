namespace InterFullMarkt.Application.Features.Transfers.Commands.ProcessTransfer;

using MediatR;
using Microsoft.EntityFrameworkCore;
using InterFullMarkt.Domain.Entities;
using InterFullMarkt.Domain.ValueObjects;
using InterFullMarkt.Domain.Events;
using InterFullMarkt.Application.Abstractions;
using Microsoft.Extensions.Logging;

public sealed class ProcessTransferCommandHandler : IRequestHandler<ProcessTransferCommand, ProcessTransferResult>
{
    private readonly IDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessTransferCommandHandler> _logger;

    public ProcessTransferCommandHandler(IDbContext dbContext, IMediator mediator, ILogger<ProcessTransferCommandHandler> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessTransferResult> Handle(ProcessTransferCommand request, CancellationToken cancellationToken)
    {
        var player = await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == request.PlayerId, cancellationToken);
        var fromClub = await _dbContext.Clubs.Include(c => c.Players).FirstOrDefaultAsync(c => c.Id == request.FromClubId, cancellationToken);
        var toClub = await _dbContext.Clubs.Include(c => c.Players).FirstOrDefaultAsync(c => c.Id == request.ToClubId, cancellationToken);

        if (player is null || fromClub is null || toClub is null)
            return ProcessTransferResult.Failure("Transfer için gerekli (Oyuncu, Eski Kulüp, Yeni Kulüp) varlıklardan biri bulunamadı.");

        var fee = Money.Create(request.FeeAmount, request.Currency);
        
        var transfer = new Transfer(
            fromClubId: fromClub.Id,
            toClubId: toClub.Id,
            playerId: player.Id,
            fee: fee,
            transferDate: request.TransferDate,
            transferType: request.TransferType
        );

        try
        {
            // 🔥 Rich Domain Model Business Logic Execution
            transfer.CompleteTransfer(player, fromClub, toClub);

            _dbContext.Transfers.Add(transfer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 📡 Domain Event Fırlatma (AI Forecast güncellemelerini, Cache invalidasyonunu tetikler)
            await _mediator.Publish(
                new TransferCompletedEvent(transfer.Id, player.Id, fromClub.Id, toClub.Id, request.FeeAmount, request.Currency), 
                cancellationToken);

            _logger.LogInformation("Transfer başarıyla tamamlandı: {TransferId}", transfer.Id);
            return ProcessTransferResult.Success(transfer.Id, "Transfer başarıyla tamamlandı ve veri tabanına işlendi!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transfer sırasında kritik hata!");
            return ProcessTransferResult.Failure($"Transfer başarısız: {ex.Message}");
        }
    }
}