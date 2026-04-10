namespace InterFullMarkt.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InterFullMarkt.Domain.Entities;

/// <summary>
/// Club varlığının Entity Type Configuration'ı.
/// Kullüp tablosu, sütun özellikleri ve navigasyon ilişkilerini tanımlar.
/// Money Value Object Owned Type olarak yapılandırılır.
/// </summary>
public sealed class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        // Tablo adı
        builder.ToTable("Clubs", "dbo");

        // Primary Key
        builder.HasKey(c => c.Id);

        // Gerekli (Required) Alanlar
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("TEXT");

        builder.Property(c => c.ShortName)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnType("TEXT");

        builder.Property(c => c.FoundingYear)
            .IsRequired()
            .HasColumnType("INTEGER");

        builder.Property(c => c.StadiumName)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("TEXT");

        builder.Property(c => c.Colors)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("TEXT");

        builder.Property(c => c.LeagueId)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(c => c.LogoUrl)
            .HasMaxLength(500)
            .HasColumnType("TEXT");

        builder.Property(c => c.Description)
            .HasColumnType("TEXT");

        // Audit Alanları
        builder.Property(c => c.CreatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(c => c.UpdatedDate)
            .IsRequired()
            .HasColumnType("TEXT");

        builder.Property(c => c.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        builder.Property(c => c.UpdatedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Soft Delete Alanları
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnType("BOOLEAN");

        builder.Property(c => c.DeletedDate)
            .HasColumnType("TEXT");

        builder.Property(c => c.DeletedByUserId)
            .HasMaxLength(250)
            .HasColumnType("TEXT");

        // Value Object: Money (Owned Type)
        builder.OwnsOne(c => c.Budget, b =>
        {
            b.Property(m => m.Amount)
                .HasColumnName("Budget_Amount")
                .HasColumnType("REAL");

            b.Property(m => m.Currency)
                .HasColumnName("Budget_Currency")
                .HasMaxLength(3)
                .HasColumnType("TEXT");

            // Budget null olabilir
            b.IsRequired(false);
        });

        // Navigasyon İlişkileri
        builder.HasOne(c => c.League)
            .WithMany(l => l.Clubs)
            .HasForeignKey(c => c.LeagueId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(c => c.Players)
            .WithOne(p => p.CurrentClub)
            .HasForeignKey(p => p.CurrentClubId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasMany(c => c.OutgoingTransfers)
            .WithOne(t => t.FromClub)
            .HasForeignKey(t => t.FromClubId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.IncomingTransfers)
            .WithOne(t => t.ToClub)
            .HasForeignKey(t => t.ToClubId)
            .OnDelete(DeleteBehavior.Restrict);

        // İndeksler
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.ShortName).IsUnique();
        builder.HasIndex(c => c.LeagueId);
        builder.HasIndex(c => c.IsDeleted);
    }
}
