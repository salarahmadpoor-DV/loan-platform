using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessProductConfiguration : IEntityTypeConfiguration<BusinessProduct>
{
    public void Configure(EntityTypeBuilder<BusinessProduct> builder)
    {
        builder.ToTable("BusinessProducts", t =>
        {
            t.HasCheckConstraint("CK_BusinessProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)");
            t.HasCheckConstraint("CK_BusinessProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)");
            t.HasCheckConstraint("CK_BusinessProducts_Price", "[Price] IS NULL OR [Price]>=(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_BusinessProducts");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.BusinessId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.MinOrderQuantity)
            .HasPrecision(18, 3);


        builder.HasOne(x => x.Business)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessProducts_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.BusinessProducts)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_BusinessProducts_Products")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BusinessId, x.ProductId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_BusinessProducts");
    }
}
