namespace InterFullMarkt.Application.Features.Transfers.Commands.ProcessTransfer;

using MediatR;

/// <summary>
/// Oyuncuyu bir kulüpten diğerine transfer etmek için kullanılan komut.
/// </summary>
public sealed record ProcessTransferCommand(
    Guid PlayerId,
    Guid FromClubId,
    Guid ToClubId,
    decimal FeeAmount,
    string Currency,
    DateTime TransferDate,
    string TransferType = "Permanent") : IRequest<ProcessTransferResult>;

/// <summary>
/// Transfer işleminin sonucu
/// </summary>
public sealed class ProcessTransferResult
{
    public required bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public Guid? TransferId { get; set; }

    public static ProcessTransferResult Success(Guid transferId, string message) =>
        new() { 
            IsSuccess = true, 
            TransferId = transferId, 
            Message = message 
        };

    public static ProcessTransferResult Failure(string message) =>
        new() { 
            IsSuccess = false, 
            Message = message 
        };
}