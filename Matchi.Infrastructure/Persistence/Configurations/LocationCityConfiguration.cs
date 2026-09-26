using Matchi.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class LocationCityConfiguration : IEntityTypeConfiguration<LocationCity>
{
    public void Configure(EntityTypeBuilder<LocationCity> builder)
    {
        builder.ToTable("LocationCities");

        builder.HasKey(x => x.Id).HasName("PK_LocationCities");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureCreateDate("DF_LocationCities_CreateDate");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("([Code]<>N'')")
            .HasDatabaseName("UX_LocationCities_Code");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CenterLat)
            .HasPrecision(9, 6);

        builder.Property(x => x.CenterLng)
            .HasPrecision(9, 6);

        builder.Property(x => x.RadiusKm)
            .HasPrecision(8, 2);

        builder.HasOne(x => x.Province)
            .WithMany(x => x.Cities)
            .HasForeignKey(x => x.ProvinceId)
            .HasConstraintName("FK_LocationCities_LocationProvinces_ProvinceId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ProvinceId)
            .HasDatabaseName("IX_LocationCities_ProvinceId");
    }
}
