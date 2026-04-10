namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
    
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; } = null!;
    public Money TotalPrice { get; set; } = null!;

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid productId, int quantity, Money unitPrice) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = Money.Create(unitPrice.Amount * quantity, unitPrice.Currency);
    }
}
