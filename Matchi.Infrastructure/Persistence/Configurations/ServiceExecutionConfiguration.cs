using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ServiceExecutionConfiguration : IEntityTypeConfiguration<ServiceExecution>
{
    public void Configure(EntityTypeBuilder<ServiceExecution> builder)
    {
        builder.ToTable("ServiceExecutions", t =>
        {
            t.HasCheckConstraint("CK_ServiceExecutions_Time", "[ScheduledTimeFrom] IS NULL OR [ScheduledTimeTo] IS NULL OR [ScheduledTimeFrom]<[ScheduledTimeTo]");
        });

        builder.HasKey(x => x.Id).HasName("PK_ServiceExecutions");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.DealId)
            .IsRequired();


        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Pending")
            .IsConcurrencyToken();

        builder.Property(x => x.ScheduledDate)
            .HasColumnType("date");

        builder.Property(x => x.ScheduledTimeFrom)
            .HasColumnType("time");

        builder.Property(x => x.ScheduledTimeTo)
            .HasColumnType("time");



        builder.HasOne(x => x.Business)
            .WithMany(x => x.ServiceExecutions)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_ServiceExecutions_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Deal)
            .WithMany(x => x.ServiceExecutions)
            .HasForeignKey(x => x.DealId)
            .HasConstraintName("FK_ServiceExecutions_Deals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.WithoutSingleColumnIndex(nameof(ServiceExecution.BusinessId));
        builder.WithoutSingleColumnIndex(nameof(ServiceExecution.DealId));
        builder.HasIndex(x => x.DealId)
            .IsUnique()
            .HasDatabaseName("UX_ServiceExecutions_DealId");
    }
}
