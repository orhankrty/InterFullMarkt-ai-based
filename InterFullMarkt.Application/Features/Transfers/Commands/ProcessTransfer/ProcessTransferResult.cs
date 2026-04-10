namespace InterFullMarkt.Application.Features.Transfers.Commands.ProcessTransfer;

/// <summary>
/// Result of processing a transfer
/// </summary>
public sealed class ProcessTransferResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    
    public Guid TransferId { get; set; }
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    
    public string FromClubName { get; set; } = string.Empty;
    public string ToClubName { get; set; } = string.Empty;
    
    public string TransferFee { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
}
