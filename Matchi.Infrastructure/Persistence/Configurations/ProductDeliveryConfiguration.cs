using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProductDeliveryConfiguration : IEntityTypeConfiguration<ProductDelivery>
{
    public void Configure(EntityTypeBuilder<ProductDelivery> builder)
    {
        builder.ToTable("ProductDeliveries");

        builder.HasKey(x => x.Id).HasName("PK_ProductDeliveries");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.DealId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Pending");

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .HasMaxLength(100);

        builder.Property(x => x.Lat)
            .HasPrecision(9, 6);

        builder.Property(x => x.Lng)
            .HasPrecision(9, 6);

        builder.Property(x => x.ScheduledDate)
            .HasColumnType("date");


        builder.Property(x => x.TrackingCode)
            .HasMaxLength(200);

        builder.HasOne(x => x.Deal)
            .WithMany(x => x.ProductDeliveries)
            .HasForeignKey(x => x.DealId)
            .HasConstraintName("FK_ProductDeliveries_Deals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.DealId)
            .HasDatabaseName("IX_ProductDeliveries_DealId");
    }
}
