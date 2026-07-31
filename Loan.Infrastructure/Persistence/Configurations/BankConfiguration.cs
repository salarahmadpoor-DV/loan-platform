using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Loan.Infrastructure.Persistence.Configurations;

public sealed class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.ToTable("Banks", "Loan");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(x => x.ShortDescription)
            .HasMaxLength(500);

        builder.Property(x => x.Description);

        builder.Property(x => x.TelegramLink)
            .HasMaxLength(300);

        builder.Property(x => x.EitaaLink)
            .HasMaxLength(300);

        builder.Property(x => x.RubikaLink)
            .HasMaxLength(300);

        builder.Property(x => x.SeoTitle)
            .HasMaxLength(200);

        builder.Property(x => x.SeoDescription)
            .HasMaxLength(500);

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(x => x.CreateDate)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.UpdateDate)
            .HasColumnType("datetime2(0)");

        // Indexes
        builder.HasIndex(x => x.Title)
            .IsUnique();

        builder.HasIndex(x => x.Slug)
            .IsUnique();

        builder.HasIndex(x => new
        {
            x.IsActive,
            x.SortOrder
        });

        // Navigation
        builder.HasMany(x => x.Questions)
            .WithOne()
            .HasForeignKey("BankId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.LoanRequests)
            .WithOne()
            .HasForeignKey("BankId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}