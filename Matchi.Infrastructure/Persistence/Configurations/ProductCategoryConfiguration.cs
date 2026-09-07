using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");

        builder.HasKey(x => x.Id).HasName("PK_ProductCategories");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();


        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .HasConstraintName("FK_ProductCategories_Parent")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ParentId)
            .HasDatabaseName("IX_ProductCategories_ParentId");

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ProductCategories_Slug");
    }
}
