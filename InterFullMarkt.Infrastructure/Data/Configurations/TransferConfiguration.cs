namespace InterFullMarkt.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InterFullMarkt.Domain.Entities;

/// <summary>
/// Transfer varlığının Entity Type Configuration'ı.
/// Transfer tablosu, sütun özellikleri ve navigasyon ilişkilerini tanımlar.
/// Money Value Object Owned Type olarak yapılandırılır.
/// </summary>
public sealed class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> builder)
    {
        // Tablo adı
        builder.ToTable("Transfers", "dbo");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Gerekli (Required) Alanlar
        builder.Property(t => t.FromClubId)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.ToClubId)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.PlayerId)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.TransferDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.TransferType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Permanent")
            .HasColumnType("TEXT");

        builder.Property(t => t.Notes)
            .HasColumnType("TEXT");

        // Audit Alanları
        builder.Property(t => t.CreatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.UpdatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(t => t.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        builder.Property(t => t.UpdatedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Soft Delete Alanları
        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("BOOLEAN");

        builder.Property(t => t.DeletedDate)
            .HasColumnType("TEXT");

        builder.Property(t => t.DeletedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Value Object: Money (Owned Type) - Transfer Bedeli (Fee)
        builder.OwnsOne(t => t.Fee, f =>
        {
            f.Property(m => m.Amount)
                .HasColumnName("Fee_Amount")
                .IsRequired()
                .HasColumnType("REAL");

            f.Property(m => m.Currency)
                .HasColumnName("Fee_Currency")
                .IsRequired()
                .HasMaxLength(3)
                .HasColumnType("TEXT");
        });

        // Navigasyon İlişkileri
        builder.HasOne(t => t.FromClub)
            .WithMany(c => c.OutgoingTransfers)
            .HasForeignKey(t => t.FromClubId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(t => t.ToClub)
            .WithMany(c => c.IncomingTransfers)
            .HasForeignKey(t => t.ToClubId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(t => t.Player)
            .WithMany(p => p.TransfersAsPlayer)
            .HasForeignKey(t => t.PlayerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // İndeksler
        builder.HasIndex(t => t.FromClubId);
        builder.HasIndex(t => t.ToClubId);
        builder.HasIndex(t => t.PlayerId);
        builder.HasIndex(t => t.TransferDate);
        builder.HasIndex(t => t.IsDeleted);
        
        // Composite Index: Aynı oyuncu aynı gün içinde yalnızca bir transfer
        builder.HasIndex(t => new { t.PlayerId, t.TransferDate }).IsUnique();
    }
}
