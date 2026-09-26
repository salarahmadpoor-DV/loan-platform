using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestLocationConfiguration : IEntityTypeConfiguration<RequestLocation>
{
    public void Configure(EntityTypeBuilder<RequestLocation> builder)
    {
        builder.ToTable("RequestLocations");

        builder.HasKey(x => x.Id).HasName("PK_RequestLocations");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.RequestId)
            .IsRequired();

        builder.Property(x => x.ProvinceId);

        builder.Property(x => x.CityId);

        builder.Property(x => x.DistrictId);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        // Legacy text columns retained until a data-migration strategy is confirmed.
        builder.Property<string?>("LegacyProvince")
            .HasColumnName("Province")
            .HasMaxLength(100);

        builder.Property<string?>("LegacyCity")
            .HasColumnName("City")
            .HasMaxLength(100);

        builder.Property<string?>("LegacyDistrict")
            .HasColumnName("District")
            .HasMaxLength(100);

        builder.Property(x => x.Lat)
            .HasPrecision(9, 6);

        builder.Property(x => x.Lng)
            .HasPrecision(9, 6);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Locations)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_RequestLocations_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Province)
            .WithMany()
            .HasForeignKey(x => x.ProvinceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_RequestLocations_LocationProvinces_ProvinceId");

        builder.HasOne(x => x.City)
            .WithMany()
            .HasForeignKey(x => x.CityId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_RequestLocations_LocationCities_CityId");

        builder.HasOne(x => x.District)
            .WithMany()
            .HasForeignKey(x => x.DistrictId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction)
            .HasConstraintName("FK_RequestLocations_LocationDistricts_DistrictId");

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_RequestLocations_RequestId");

        builder.HasIndex(x => x.ProvinceId)
            .HasDatabaseName("IX_RequestLocations_ProvinceId");

        builder.HasIndex(x => x.CityId)
            .HasDatabaseName("IX_RequestLocations_CityId");

        builder.HasIndex(x => x.DistrictId)
            .HasDatabaseName("IX_RequestLocations_DistrictId");
    }
}
