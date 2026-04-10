namespace InterFullMarkt.Domain.Entities;

using InterFullMarkt.Domain.Common;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Sistem kullanıcısını temsil eden varlık.
/// </summary>
public sealed class User : BaseEntity, IAuditEntity, ISoftDelete
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string Role { get; set; } = "User";

    // Profil Bilgileri
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Navigation Properties
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public string CreatedByUserId { get; set; } = string.Empty;
    public string? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedByUserId { get; set; }

    [SetsRequiredMembers]
    public User(string username, string email, string passwordHash, string role = "User") : base(Guid.NewGuid())
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    private User() { }
}