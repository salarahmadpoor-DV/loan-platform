using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessServiceConfiguration : IEntityTypeConfiguration<BusinessService>
{
    public void Configure(EntityTypeBuilder<BusinessService> builder)
    {
        builder.ToTable("BusinessServices", t =>
        {
            t.HasCheckConstraint("CK_BusinessServices_PriceRange", "[MinPrice] IS NULL OR [MaxPrice] IS NULL OR [MinPrice]<=[MaxPrice]");
        });

        builder.HasKey(x => x.Id).HasName("PK_BusinessServices");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.BusinessId)
            .IsRequired();

        builder.Property(x => x.ServiceId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CanCustomerChooseProvider)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.MinPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxPrice)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessServices_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.BusinessServices)
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_BusinessServices_Services")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BusinessId, x.ServiceId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_BusinessServices");
    }
}
