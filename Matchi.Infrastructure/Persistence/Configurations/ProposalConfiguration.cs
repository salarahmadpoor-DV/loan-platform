using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProposalConfiguration : IEntityTypeConfiguration<Proposal>
{
    public void Configure(EntityTypeBuilder<Proposal> builder)
    {
        builder.ToTable("Proposals", t =>
        {
            t.HasCheckConstraint("CK_Proposals_Party", "[BusinessId] IS NOT NULL AND [ProviderId] IS NULL OR [BusinessId] IS NULL AND [ProviderId] IS NOT NULL");
            t.HasCheckConstraint("CK_Proposals_Prices", "[TotalPrice]>=(0) AND [DeliveryFee]>=(0)");
            t.HasCheckConstraint("CK_Proposals_Time", "[ProposedTimeFrom] IS NULL OR [ProposedTimeTo] IS NULL OR [ProposedTimeFrom]<[ProposedTimeTo]");
        });

        builder.HasKey(x => x.Id).HasName("PK_Proposals");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.RequestId)
            .IsRequired();



        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DeliveryFee)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(x => x.Message)
            .HasMaxLength(2000);

        builder.Property(x => x.ProposedDate)
            .HasColumnType("date");

        builder.Property(x => x.ProposedTimeFrom)
            .HasColumnType("time");

        builder.Property(x => x.ProposedTimeTo)
            .HasColumnType("time");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Pending");


        builder.HasOne(x => x.Business)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_Proposals_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_Proposals_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Proposals)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_Proposals_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.BusinessId)
            .HasDatabaseName("IX_Proposals_BusinessId");

        builder.HasIndex(x => x.ProviderId)
            .HasDatabaseName("IX_Proposals_ProviderId");

        builder.HasIndex(x => new { x.RequestId, x.Status, x.CreateDate })
            .HasDatabaseName("IX_Proposals_RequestId_Status");
    }
}
