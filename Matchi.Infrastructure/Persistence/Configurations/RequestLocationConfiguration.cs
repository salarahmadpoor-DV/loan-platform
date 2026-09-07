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

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Locations)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_RequestLocations_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_RequestLocations_RequestId");
    }
}
