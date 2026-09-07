using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestScheduleConfiguration : IEntityTypeConfiguration<RequestSchedule>
{
    public void Configure(EntityTypeBuilder<RequestSchedule> builder)
    {
        builder.ToTable("RequestSchedules", t =>
        {
            t.HasCheckConstraint("CK_RequestSchedules_Time", "[TimeFrom] IS NULL OR [TimeTo] IS NULL OR [TimeFrom]<[TimeTo]");
        });

        builder.HasKey(x => x.Id).HasName("PK_RequestSchedules");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.RequestId)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(x => x.TimeFrom)
            .HasColumnType("time");

        builder.Property(x => x.TimeTo)
            .HasColumnType("time");

        builder.Property(x => x.IsFlexible)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Schedules)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_RequestSchedules_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.RequestId, x.Date })
            .HasDatabaseName("IX_RequestSchedules_RequestId_Date");
    }
}
