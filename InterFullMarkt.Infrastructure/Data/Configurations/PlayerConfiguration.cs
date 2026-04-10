namespace InterFullMarkt.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InterFullMarkt.Domain.Entities;

/// <summary>
/// Player varlığının Entity Type Configuration'ı.
/// Tablo yapısı, sütun özellikleri ve navigasyon ilişkilerini tanımlar.
/// Value Objects (Nationality, Money) Owned Types olarak yapılandırılır.
/// </summary>
public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        // Tablo adı
        builder.ToTable("Players", "dbo");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Gerekli (Required) Alanlar
        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnType("TEXT");

        builder.Property(p => p.Position)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType("INTEGER");

        builder.Property(p => p.DateOfBirth)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(p => p.Height)
            .IsRequired()
            .HasColumnType("INTEGER");

        builder.Property(p => p.Weight)
            .IsRequired()
            .HasColumnType("REAL");

        builder.Property(p => p.PreferredFoot)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("Right")
            .HasColumnType("TEXT");

        builder.Property(p => p.JerseyNumber)
            .HasColumnType("INTEGER");

        builder.Property(p => p.CurrentClubId)
            .HasColumnType("TEXT");

        // Audit Alanları
        builder.Property(p => p.CreatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(p => p.UpdatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(p => p.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        builder.Property(p => p.UpdatedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Soft Delete Alanları
        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("BOOLEAN");

        builder.Property(p => p.DeletedDate)
            .HasColumnType("TEXT");

        builder.Property(p => p.DeletedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Value Object: Nationality (Owned Type)
        builder.OwnsOne(p => p.Nationality, na =>
        {
            na.Property(n => n.CountryName)
                .HasColumnName("Nationality_CountryName")
                .IsRequired()
                .HasMaxLength(100);

            na.Property(n => n.CountryCode)
                .HasColumnName("Nationality_CountryCode")
                .IsRequired()
                .HasMaxLength(2);

            na.Property(n => n.FlagEmoji)
                .HasColumnName("Nationality_FlagEmoji")
                .IsRequired()
                .HasMaxLength(10);
        });

        // Value Object: Money (Owned Type)
        builder.OwnsOne(p => p.MarketValue, mv =>
        {
            mv.Property(m => m.Amount)
                .HasColumnName("MarketValue_Amount")
                .HasColumnType("REAL");

            mv.Property(m => m.Currency)
                .HasColumnName("MarketValue_Currency")
                .HasMaxLength(3)
                .HasColumnType("TEXT");

            // Money null olabilir
            mv.IsRequired(false);
        });

        // Navigasyon İlişkileri
        builder.HasOne(p => p.CurrentClub)
            .WithMany(c => c.Players)
            .HasForeignKey(p => p.CurrentClubId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasMany(p => p.TransfersAsPlayer)
            .WithOne(t => t.Player)
            .HasForeignKey(t => t.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);

        // İndeksler
        builder.HasIndex(p => p.FullName);
        builder.HasIndex(p => p.CurrentClubId);
        builder.HasIndex(p => p.IsDeleted);
    }
}
