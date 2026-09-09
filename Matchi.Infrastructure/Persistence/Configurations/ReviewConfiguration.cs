using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews", t =>
        {
            t.HasCheckConstraint("CK_Reviews_Rating", "[Rating]>=(1) AND [Rating]<=(5)");
            t.HasCheckConstraint("CK_Reviews_Target", "[BusinessId] IS NOT NULL OR [ProviderId] IS NOT NULL");
        });

        builder.HasKey(x => x.Id).HasName("PK_Reviews");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.DealId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .IsRequired();



        builder.Property(x => x.Rating)
            .IsRequired()
            .HasColumnType("tinyint");

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_Reviews_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Reviews_Customers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Deal)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.DealId)
            .HasConstraintName("FK_Reviews_Deals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_Reviews_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.WithoutSingleColumnIndex(nameof(Review.BusinessId));
        builder.WithoutSingleColumnIndex(nameof(Review.ProviderId));
        builder.WithoutSingleColumnIndex(nameof(Review.CustomerId));
        builder.WithoutSingleColumnIndex(nameof(Review.DealId));
        builder.HasIndex(x => x.DealId)
            .HasDatabaseName("IX_Reviews_DealId");
        builder.HasIndex(x => new { x.DealId, x.CustomerId, x.BusinessId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [BusinessId] IS NOT NULL")
            .HasDatabaseName("UX_Reviews_Deal_Customer_Business");
        builder.HasIndex(x => new { x.DealId, x.CustomerId, x.ProviderId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0 AND [ProviderId] IS NOT NULL")
            .HasDatabaseName("UX_Reviews_Deal_Customer_Provider");
    }
}
