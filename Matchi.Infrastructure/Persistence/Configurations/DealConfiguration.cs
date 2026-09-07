using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals", t =>
        {
            t.HasCheckConstraint("CK_Deals_TotalPrice", "[TotalPrice]>=(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_Deals");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestId)
            .IsRequired();

        builder.Property(x => x.ProposalId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Active");

        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.AcceptedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");



        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Deals_Customers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Proposal)
            .WithOne(x => x.Deal)
            .HasForeignKey<Deal>(x => x.ProposalId)
            .HasConstraintName("FK_Deals_Proposals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Deals)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_Deals_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.CustomerId, x.Status })
            .HasDatabaseName("IX_Deals_CustomerId_Status");

        builder.HasIndex(x => new { x.RequestId, x.Status })
            .HasDatabaseName("IX_Deals_RequestId_Status");

        builder.HasIndex(x => x.ProposalId)
            .IsUnique()
            .HasDatabaseName("UX_Deals_ProposalId");
    }
}
