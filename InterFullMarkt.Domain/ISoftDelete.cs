namespace InterFullMarkt.Domain;

/// <summary>
/// Soft Delete (mantıksal silme) desteği sağlayan varlıklar için arayüz.
/// Veriler fiziksel olarak silinmez, sadece silinmiş olarak işaretlenir.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Varlığın silinmiş olup olmadığını belirtir
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Soft delete tarihini belirtir (nullable, henüz silinmemişse null)
    /// </summary>
    DateTime? DeletedDate { get; set; }

    /// <summary>
    /// Varlığı silen kullanıcı ID'si (nullable)
    /// </summary>
    string? DeletedByUserId { get; set; }
}
