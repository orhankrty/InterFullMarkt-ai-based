namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using InterFullMarkt.Domain.ValueObjects;

public sealed class Product : BaseEntity, IAuditEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Money Price { get; set; } = null!;
    public string Category { get; set; } = string.Empty; // Forma, Krampon, Aksesuar
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public bool IsFeatured { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    private Product() { }

    public Product(string name, string description, Money price, string category, string? imageUrl, int stockQuantity) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        Price = price;
        Category = category;
        ImageUrl = imageUrl;
        StockQuantity = stockQuantity;
    }
}
