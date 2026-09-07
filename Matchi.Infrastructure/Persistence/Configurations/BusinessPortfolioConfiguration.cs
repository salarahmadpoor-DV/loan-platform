using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessPortfolioConfiguration : IEntityTypeConfiguration<BusinessPortfolio>
{
    public void Configure(EntityTypeBuilder<BusinessPortfolio> builder)
    {
        builder.ToTable("BusinessPortfolios");

        builder.HasKey(x => x.Id).HasName("PK_BusinessPortfolios");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(x => x.BusinessId)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Portfolios)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessPortfolios_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.BusinessId)
            .HasDatabaseName("IX_BusinessPortfolios_BusinessId");
    }
}
