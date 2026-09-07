using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestProductAttributeConfiguration : IEntityTypeConfiguration<RequestProductAttribute>
{
    public void Configure(EntityTypeBuilder<RequestProductAttribute> builder)
    {
        builder.ToTable("RequestProductAttributes");

        builder.HasKey(x => x.Id).HasName("PK_RequestProductAttributes");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestProductId)
            .IsRequired();

        builder.Property(x => x.ProductAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(2000);

        builder.HasOne(x => x.ProductAttribute)
            .WithMany(x => x.RequestProductAttributes)
            .HasForeignKey(x => x.ProductAttributeId)
            .HasConstraintName("FK_RequestProductAttributes_ProductAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.RequestProduct)
            .WithMany(x => x.Attributes)
            .HasForeignKey(x => x.RequestProductId)
            .HasConstraintName("FK_RequestProductAttributes_RequestProducts")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.RequestProductId, x.ProductAttributeId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_RequestProductAttributes");
    }
}
