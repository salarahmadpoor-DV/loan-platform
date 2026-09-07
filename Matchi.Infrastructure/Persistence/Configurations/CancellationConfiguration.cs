using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class CancellationConfiguration : IEntityTypeConfiguration<Cancellation>
{
    public void Configure(EntityTypeBuilder<Cancellation> builder)
    {
        builder.ToTable("Cancellations");

        builder.HasKey(x => x.Id).HasName("PK_Cancellations");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(x => x.RequestId)
            .IsRequired();


        builder.Property(x => x.CancelledByUserId)
            .IsRequired();

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasOne(x => x.Deal)
            .WithMany(x => x.Cancellations)
            .HasForeignKey(x => x.DealId)
            .HasConstraintName("FK_Cancellations_Deals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Cancellations)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_Cancellations_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CancelledByUser)
            .WithMany(x => x.Cancellations)
            .HasForeignKey(x => x.CancelledByUserId)
            .HasConstraintName("FK_Cancellations_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.DealId)
            .HasDatabaseName("IX_Cancellations_DealId");

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_Cancellations_RequestId");
    }
}
