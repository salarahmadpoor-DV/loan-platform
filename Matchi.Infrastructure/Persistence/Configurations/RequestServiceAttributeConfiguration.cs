using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestServiceAttributeConfiguration : IEntityTypeConfiguration<RequestServiceAttribute>
{
    public void Configure(EntityTypeBuilder<RequestServiceAttribute> builder)
    {
        builder.ToTable("RequestServiceAttributes");

        builder.HasKey(x => x.Id).HasName("PK_RequestServiceAttributes");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestServiceId)
            .IsRequired();

        builder.Property(x => x.ServiceAttributeId)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(2000);

        builder.HasOne(x => x.RequestService)
            .WithMany(x => x.Attributes)
            .HasForeignKey(x => x.RequestServiceId)
            .HasConstraintName("FK_RequestServiceAttributes_RequestServices")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ServiceAttribute)
            .WithMany(x => x.RequestServiceAttributes)
            .HasForeignKey(x => x.ServiceAttributeId)
            .HasConstraintName("FK_RequestServiceAttributes_ServiceAttributes")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.RequestServiceId, x.ServiceAttributeId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_RequestServiceAttributes");
    }
}
