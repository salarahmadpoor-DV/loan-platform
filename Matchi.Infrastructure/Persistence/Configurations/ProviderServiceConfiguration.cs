using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderServiceConfiguration : IEntityTypeConfiguration<ProviderService>
{
    public void Configure(EntityTypeBuilder<ProviderService> builder)
    {
        builder.ToTable("ProviderServices");

        builder.HasKey(x => x.Id).HasName("PK_ProviderServices");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.ServiceId)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.ProviderServices)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderServices_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.ProviderServices)
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_ProviderServices_Services")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.ServiceId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ProviderServices");
    }
}
