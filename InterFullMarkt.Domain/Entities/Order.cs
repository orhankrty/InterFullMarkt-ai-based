namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

public sealed class Order : BaseEntity, IAuditEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public Money TotalAmount { get; set; } = null!;
    public string Status { get; set; } = "Pending"; // Pending, Preparing, Shipped, Completed, Cancelled
    
    public string ShippingAddress { get; set; } = string.Empty;
    public string? OrderNote { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    private Order() { }

    public Order(Guid userId, Money totalAmount, string shippingAddress) : base(Guid.NewGuid())
    {
        UserId = userId;
        TotalAmount = totalAmount;
        ShippingAddress = shippingAddress;
        OrderDate = DateTime.UtcNow;
    }
}
