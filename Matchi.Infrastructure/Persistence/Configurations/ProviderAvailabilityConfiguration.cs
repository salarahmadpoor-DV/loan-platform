using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderAvailabilityConfiguration : IEntityTypeConfiguration<ProviderAvailability>
{
    public void Configure(EntityTypeBuilder<ProviderAvailability> builder)
    {
        builder.ToTable("ProviderAvailabilities", t =>
        {
            t.HasCheckConstraint("CK_ProviderAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)");
            t.HasCheckConstraint("CK_ProviderAvailabilities_Time", "[TimeFrom]<[TimeTo]");
        });

        builder.HasKey(x => x.Id).HasName("PK_ProviderAvailabilities");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.DayOfWeek)
            .IsRequired()
            .HasColumnType("tinyint");

        builder.Property(x => x.TimeFrom)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(x => x.TimeTo)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(x => x.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Availabilities)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ProviderAvailabilities_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.DayOfWeek })
            .HasDatabaseName("IX_ProviderAvailabilities");
    }
}
