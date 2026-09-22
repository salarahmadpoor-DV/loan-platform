using Matchi.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class LocationDistrictConfiguration : IEntityTypeConfiguration<LocationDistrict>
{
    public void Configure(EntityTypeBuilder<LocationDistrict> builder)
    {
        builder.ToTable("LocationDistricts");

        builder.HasKey(x => x.Id).HasName("PK_LocationDistricts");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureCreateDate("DF_LocationDistricts_CreateDate");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("([Code]<>N'')")
            .HasDatabaseName("UX_LocationDistricts_Code");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CenterLat)
            .HasPrecision(9, 6);

        builder.Property(x => x.CenterLng)
            .HasPrecision(9, 6);

        builder.Property(x => x.RadiusKm)
            .HasPrecision(8, 2);

        builder.HasOne(x => x.City)
            .WithMany(x => x.Districts)
            .HasForeignKey(x => x.CityId)
            .HasConstraintName("FK_LocationDistricts_LocationCities_CityId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.CityId)
            .HasDatabaseName("IX_LocationDistricts_CityId");
    }
}
