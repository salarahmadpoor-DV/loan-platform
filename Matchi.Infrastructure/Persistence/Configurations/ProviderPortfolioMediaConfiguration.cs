using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderPortfolioMediaConfiguration : IEntityTypeConfiguration<ProviderPortfolioMedia>
{
    public void Configure(EntityTypeBuilder<ProviderPortfolioMedia> builder)
    {
        builder.ToTable("ProviderPortfolioMedia");

        builder.HasKey(x => x.Id).HasName("PK_ProviderPortfolioMedia");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PortfolioId)
            .IsRequired();

        builder.Property(x => x.MediaId)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Media)
            .WithMany(x => x.ProviderPortfolioMedia)
            .HasForeignKey(x => x.MediaId)
            .HasConstraintName("FK_ProviderPortfolioMedia_Media")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Portfolio)
            .WithMany(x => x.MediaItems)
            .HasForeignKey(x => x.PortfolioId)
            .HasConstraintName("FK_ProviderPortfolioMedia_Portfolios")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.PortfolioId, x.DisplayOrder })
            .HasDatabaseName("IX_ProviderPortfolioMedia_PortfolioId");
    }
}
