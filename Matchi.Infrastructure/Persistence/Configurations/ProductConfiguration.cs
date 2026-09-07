using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id).HasName("PK_Products");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.Slug)
            .HasMaxLength(300);

        builder.Property(x => x.Description)
            .HasMaxLength(4000);

        builder.Property(x => x.Brand)
            .HasMaxLength(200);

        builder.Property(x => x.Model)
            .HasMaxLength(200);

        builder.Property(x => x.SKU)
            .HasMaxLength(100);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_Products_ProductCategories")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.SKU)
            .IsUnique()
            .HasFilter("([SKU] IS NOT NULL AND [IsDeleted]=(0))")
            .HasDatabaseName("UX_Products_SKU");

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("([Slug] IS NOT NULL AND [IsDeleted]=(0))")
            .HasDatabaseName("UX_Products_Slug");
    }
}
