namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;

public sealed class Address : BaseEntity, IAuditEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public string Title { get; set; } = string.Empty; // e.g., "Home", "Office"
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Country { get; set; } = "Turkey";
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    
    public bool IsDefault { get; set; }

    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }

    private Address() { }

    public Address(Guid userId, string title, string firstName, string lastName, string phoneNumber, string city, string district, string addressLine) : base(Guid.NewGuid())
    {
        UserId = userId;
        Title = title;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        City = city;
        District = district;
        AddressLine = addressLine;
    }
}
