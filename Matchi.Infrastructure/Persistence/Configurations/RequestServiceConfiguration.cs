using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestServiceConfiguration : IEntityTypeConfiguration<RequestService>
{
    public void Configure(EntityTypeBuilder<RequestService> builder)
    {
        builder.ToTable("RequestServices", t =>
        {
            t.HasCheckConstraint("CK_RequestServices_Quantity", "[Quantity]>(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_RequestServices");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestId)
            .IsRequired();

        builder.Property(x => x.ServiceId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 3)
            .HasDefaultValue(1m);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_RequestServices_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.RequestServices)
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_RequestServices_Services")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_RequestServices_RequestId");

        builder.HasIndex(x => x.ServiceId)
            .HasDatabaseName("IX_RequestServices_ServiceId");
    }
}
