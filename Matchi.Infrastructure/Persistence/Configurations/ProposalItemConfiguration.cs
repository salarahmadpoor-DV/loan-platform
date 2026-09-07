using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProposalItemConfiguration : IEntityTypeConfiguration<ProposalItem>
{
    public void Configure(EntityTypeBuilder<ProposalItem> builder)
    {
        builder.ToTable("ProposalItems", t =>
        {
            t.HasCheckConstraint("CK_ProposalItems_Prices", "[UnitPrice]>=(0) AND [TotalPrice]>=(0)");
            t.HasCheckConstraint("CK_ProposalItems_Quantity", "[Quantity]>(0)");
            t.HasCheckConstraint("CK_ProposalItems_Type", "[ItemType]=N'Product' AND [ProductId] IS NOT NULL AND [ServiceId] IS NULL OR [ItemType]=N'Service' AND [ProductId] IS NULL AND [ServiceId] IS NOT NULL");
        });

        builder.HasKey(x => x.Id).HasName("PK_ProposalItems");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.ProposalId)
            .IsRequired();

        builder.Property(x => x.ItemType)
            .IsRequired()
            .HasMaxLength(20);



        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 3)
            .HasDefaultValue(1m);

        builder.Property(x => x.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProposalItems)
            .HasForeignKey(x => x.ProductId)
            .HasConstraintName("FK_ProposalItems_Products")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Proposal)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ProposalId)
            .HasConstraintName("FK_ProposalItems_Proposals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Service)
            .WithMany(x => x.ProposalItems)
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("FK_ProposalItems_Services")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_ProposalItems_ProductId");

        builder.HasIndex(x => new { x.ProposalId, x.DisplayOrder })
            .HasDatabaseName("IX_ProposalItems_ProposalId");

        builder.HasIndex(x => x.ServiceId)
            .HasDatabaseName("IX_ProposalItems_ServiceId");
    }
}
