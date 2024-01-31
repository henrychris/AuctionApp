using AuctionApp.Domain.Entities.Base;
using AuctionApp.Domain.Enums;

namespace AuctionApp.Domain.Entities;

public class Payment : BaseEntity
{
    public PaymentStatus Status { get; set; } = PaymentStatus.Incomplete;
    public required string InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    
    public bool IsCompleted => Status == PaymentStatus.Complete;
    
    public void Complete()
    {
        Status = PaymentStatus.Complete;
    }
}