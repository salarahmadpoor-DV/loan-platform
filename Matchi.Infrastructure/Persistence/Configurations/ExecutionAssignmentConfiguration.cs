using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ExecutionAssignmentConfiguration : IEntityTypeConfiguration<ExecutionAssignment>
{
    public void Configure(EntityTypeBuilder<ExecutionAssignment> builder)
    {
        builder.ToTable("ExecutionAssignments");

        builder.HasKey(x => x.Id).HasName("PK_ExecutionAssignments");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.ServiceExecutionId)
            .IsRequired();

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IsPrimary)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Assigned");

        builder.Property(x => x.AssignedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");



        builder.HasOne(x => x.Provider)
            .WithMany(x => x.ExecutionAssignments)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_ExecutionAssignments_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ServiceExecution)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.ServiceExecutionId)
            .HasConstraintName("FK_ExecutionAssignments_ServiceExecutions")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ProviderId, x.Status })
            .HasDatabaseName("IX_ExecutionAssignments_ProviderId");
    }
}
