using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderCapabilityConfiguration : IEntityTypeConfiguration<ProviderCapability>
{
    public void Configure(EntityTypeBuilder<ProviderCapability> builder)
    {
        builder.ToTable("ProviderCapabilities");

        builder.HasKey(x => x.Id).HasName("PK_ProviderCapabilities");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.ServiceAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Capabilities)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderCapabilities_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ServiceAttribute)
            .WithMany(x => x.ProviderCapabilities)
            .HasForeignKey(x => x.ServiceAttributeId)
            .HasConstraintName("FK_ProviderCapabilities_ServiceAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.ServiceAttributeId })
            .HasDatabaseName("IX_ProviderCapabilities");
    }
}
