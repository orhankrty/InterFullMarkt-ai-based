namespace InterFullMarkt.Application.Features.Transfers.Commands.ProcessTransfer;

using MediatR;

/// <summary>
/// Oyuncu transferini işleyen komut
/// </summary>
public sealed record ProcessTransferCommand(
    Guid PlayerId,
    Guid FromClubId,
    Guid ToClubId,
    decimal TransferFee,
    string Currency = "EUR",
    string? Notes = null
) : IRequest<ProcessTransferResult>;
