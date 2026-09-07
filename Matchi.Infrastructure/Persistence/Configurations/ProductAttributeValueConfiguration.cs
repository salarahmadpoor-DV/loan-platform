using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");

        builder.HasKey(x => x.Id).HasName("PK_ProductAttributeValues");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.ProductAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.ProductAttributeId)
            .HasConstraintName("FK_ProductAttributeValues_ProductAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.AttributeValues)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_ProductAttributeValues_Products")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProductId, x.ProductAttributeId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ProductAttributeValues_ProductAttribute");
    }
}
