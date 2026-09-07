using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProductAttributeOptionConfiguration : IEntityTypeConfiguration<ProductAttributeOption>
{
    public void Configure(EntityTypeBuilder<ProductAttributeOption> builder)
    {
        builder.ToTable("ProductAttributeOptions");

        builder.HasKey(x => x.Id).HasName("PK_ProductAttributeOptions");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProductAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.ProductAttributeId)
            .HasConstraintName("FK_ProductAttributeOptions_ProductAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProductAttributeId, x.Value })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ProductAttributeOptions_Value");
    }
}
