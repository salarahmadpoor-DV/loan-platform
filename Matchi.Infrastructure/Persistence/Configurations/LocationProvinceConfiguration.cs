using Matchi.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class LocationProvinceConfiguration : IEntityTypeConfiguration<LocationProvince>
{
    public void Configure(EntityTypeBuilder<LocationProvince> builder)
    {
        builder.ToTable("LocationProvinces");

        builder.HasKey(x => x.Id).HasName("PK_LocationProvinces");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureCreateDate("DF_LocationProvinces_CreateDate");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("([Code]<>N'')")
            .HasDatabaseName("UX_LocationProvinces_Code");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
