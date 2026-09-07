using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessServiceAreaConfiguration : IEntityTypeConfiguration<BusinessServiceArea>
{
    public void Configure(EntityTypeBuilder<BusinessServiceArea> builder)
    {
        builder.ToTable("BusinessServiceAreas");

        builder.HasKey(x => x.Id).HasName("PK_BusinessServiceAreas");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.BusinessId)
            .IsRequired();

        builder.Property(x => x.AreaType)
            .IsRequired()
            .HasMaxLength(30);

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

        builder.Property(x => x.Radius)
            .HasPrecision(10, 2);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.ServiceAreas)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessServiceAreas_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BusinessId, x.City, x.District })
            .HasDatabaseName("IX_BusinessServiceAreas");
    }
}
