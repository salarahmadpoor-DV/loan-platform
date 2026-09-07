using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderServiceAreaConfiguration : IEntityTypeConfiguration<ProviderServiceArea>
{
    public void Configure(EntityTypeBuilder<ProviderServiceArea> builder)
    {
        builder.ToTable("ProviderServiceAreas");

        builder.HasKey(x => x.Id).HasName("PK_ProviderServiceAreas");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProviderId)
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

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.ServiceAreas)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderServiceAreas_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.City, x.District })
            .HasDatabaseName("IX_ProviderServiceAreas");
    }
}
