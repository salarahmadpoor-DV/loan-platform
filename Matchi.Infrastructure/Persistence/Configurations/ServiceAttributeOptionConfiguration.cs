using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ServiceAttributeOptionConfiguration : IEntityTypeConfiguration<ServiceAttributeOption>
{
    public void Configure(EntityTypeBuilder<ServiceAttributeOption> builder)
    {
        builder.ToTable("ServiceAttributeOptions");

        builder.HasKey(x => x.Id).HasName("PK_ServiceAttributeOptions");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ServiceAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.ServiceAttribute)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.ServiceAttributeId)
            .HasConstraintName("FK_ServiceAttributeOptions_ServiceAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ServiceAttributeId, x.Value })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ServiceAttributeOptions_Value");
    }
}
