namespace InterFullMarkt.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InterFullMarkt.Domain.Entities;

/// <summary>
/// League varlığının Entity Type Configuration'ı.
/// Lig tablosu, sütun özellikleri ve navigasyon ilişkilerini tanımlar.
/// Nationality Value Object Owned Type olarak yapılandırılır.
/// </summary>
public sealed class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> builder)
    {
        // Tablo adı
        builder.ToTable("Leagues", "dbo");

        // Primary Key
        builder.HasKey(l => l.Id);

        // Gerekli (Required) Alanlar
        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("TEXT");

        builder.Property(l => l.Tier)
            .IsRequired()
            .HasColumnType("INTEGER");

        builder.Property(l => l.Coefficient)
            .IsRequired()
            .HasColumnType("REAL");

        builder.Property(l => l.Description)
            .HasColumnType("TEXT");

        builder.Property(l => l.LogoUrl)
            .HasMaxLength(500)
            .HasColumnType("TEXT");

        // Audit Alanları
        builder.Property(l => l.CreatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(l => l.UpdatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(l => l.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        builder.Property(l => l.UpdatedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Soft Delete Alanları
        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("BOOLEAN");

        builder.Property(l => l.DeletedDate)
            .HasColumnType("TEXT");

        builder.Property(l => l.DeletedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Value Object: Nationality (Owned Type)
        builder.OwnsOne(l => l.Country, na =>
        {
            na.Property(n => n.CountryName)
                .HasColumnName("Country_CountryName")
                .IsRequired()
                .HasMaxLength(100);

            na.Property(n => n.CountryCode)
                .HasColumnName("Country_CountryCode")
                .IsRequired()
                .HasMaxLength(2);

            na.Property(n => n.FlagEmoji)
                .HasColumnName("Country_FlagEmoji")
                .IsRequired()
                .HasMaxLength(10);
        });

        // Navigasyon İlişkileri
        builder.HasMany(l => l.Clubs)
            .WithOne(c => c.League)
            .HasForeignKey(c => c.LeagueId)
            .OnDelete(DeleteBehavior.Restrict);

        // İndeksler
        builder.HasIndex(l => l.Name).IsUnique();
        builder.HasIndex(l => l.IsDeleted);
    }
}
