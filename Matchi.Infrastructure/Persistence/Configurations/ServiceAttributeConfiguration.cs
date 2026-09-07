using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ServiceAttributeConfiguration : IEntityTypeConfiguration<ServiceAttribute>
{
    public void Configure(EntityTypeBuilder<ServiceAttribute> builder)
    {
        builder.ToTable("ServiceAttributes");

        builder.HasKey(x => x.Id).HasName("PK_ServiceAttributes");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ServiceId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DataType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.IsRequired)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.Attributes)
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_ServiceAttributes_Services")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ServiceId, x.Code })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_ServiceAttributes_Code");
    }
}
