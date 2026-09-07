using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestProductConfiguration : IEntityTypeConfiguration<RequestProduct>
{
    public void Configure(EntityTypeBuilder<RequestProduct> builder)
    {
        builder.ToTable("RequestProducts", t =>
        {
            t.HasCheckConstraint("CK_RequestProducts_Quantity", "[Quantity]>(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_RequestProducts");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestId)
            .IsRequired();



        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 3)
            .HasDefaultValue(1m);

        builder.Property(x => x.Unit)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.ProductCategory)
            .WithMany(x => x.RequestProducts)
            .HasForeignKey(x => x.ProductCategoryId)
            .HasConstraintName("FK_RequestProducts_ProductCategories")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.RequestProducts)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_RequestProducts_Products")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_RequestProducts_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ProductCategoryId)
            .HasDatabaseName("IX_RequestProducts_CategoryId");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_RequestProducts_ProductId");

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_RequestProducts_RequestId");
    }
}
