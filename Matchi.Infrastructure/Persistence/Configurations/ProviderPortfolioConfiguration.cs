using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderPortfolioConfiguration : IEntityTypeConfiguration<ProviderPortfolio>
{
    public void Configure(EntityTypeBuilder<ProviderPortfolio> builder)
    {
        builder.ToTable("ProviderPortfolios");

        builder.HasKey(x => x.Id).HasName("PK_ProviderPortfolios");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Portfolios)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderPortfolios_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ProviderId)
            .HasDatabaseName("IX_ProviderPortfolios_ProviderId");
    }
}
