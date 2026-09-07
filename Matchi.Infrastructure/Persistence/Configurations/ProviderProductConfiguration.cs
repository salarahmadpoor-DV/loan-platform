using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderProductConfiguration : IEntityTypeConfiguration<ProviderProduct>
{
    public void Configure(EntityTypeBuilder<ProviderProduct> builder)
    {
        builder.ToTable("ProviderProducts", t =>
        {
            t.HasCheckConstraint("CK_ProviderProducts_LeadTimeDays", "[LeadTimeDays] IS NULL OR [LeadTimeDays]>=(0)");
            t.HasCheckConstraint("CK_ProviderProducts_MinOrderQuantity", "[MinOrderQuantity] IS NULL OR [MinOrderQuantity]>(0)");
            t.HasCheckConstraint("CK_ProviderProducts_Price", "[Price] IS NULL OR [Price]>=(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_ProviderProducts");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProviderId)
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


        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProviderProducts)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_ProviderProducts_Products")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderProducts_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.ProductId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ProviderProducts");
    }
}
