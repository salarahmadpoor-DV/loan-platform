using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessAvailabilityConfiguration : IEntityTypeConfiguration<BusinessAvailability>
{
    public void Configure(EntityTypeBuilder<BusinessAvailability> builder)
    {
        builder.ToTable("BusinessAvailabilities", t =>
        {
            t.HasCheckConstraint("CK_BusinessAvailabilities_DayOfWeek", "[DayOfWeek]>=(0) AND [DayOfWeek]<=(6)");
            t.HasCheckConstraint("CK_BusinessAvailabilities_Time", "[TimeFrom]<[TimeTo]");
        });

        builder.HasKey(x => x.Id).HasName("PK_BusinessAvailabilities");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.BusinessId)
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

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Availabilities)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessAvailabilities_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.BusinessId, x.DayOfWeek })
            .HasDatabaseName("IX_BusinessAvailabilities");
    }
}
