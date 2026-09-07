using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessPortfolioMediaConfiguration : IEntityTypeConfiguration<BusinessPortfolioMedia>
{
    public void Configure(EntityTypeBuilder<BusinessPortfolioMedia> builder)
    {
        builder.ToTable("BusinessPortfolioMedia");

        builder.HasKey(x => x.Id).HasName("PK_BusinessPortfolioMedia");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PortfolioId)
            .IsRequired();

        builder.Property(x => x.MediaId)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Media)
            .WithMany(x => x.BusinessPortfolioMedia)
            .HasForeignKey(x => x.MediaId)
            .HasConstraintName("FK_BusinessPortfolioMedia_Media")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Portfolio)
            .WithMany(x => x.MediaItems)
            .HasForeignKey(x => x.PortfolioId)
            .HasConstraintName("FK_BusinessPortfolioMedia_Portfolios")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.PortfolioId, x.DisplayOrder })
            .HasDatabaseName("IX_BusinessPortfolioMedia_PortfolioId");
    }
}
