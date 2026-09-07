using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> builder)
    {
        builder.ToTable("Complaints");

        builder.HasKey(x => x.Id).HasName("PK_Complaints");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.RequestId)
            .IsRequired();


        builder.Property(x => x.CustomerId)
            .IsRequired();



        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Open");

        builder.Property(x => x.Resolution)
            .HasMaxLength(4000);


        builder.HasOne(x => x.Business)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_Complaints_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Complaints_Customers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Deal)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.DealId)
            .HasConstraintName("FK_Complaints_Deals")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_Complaints_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Complaints)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_Complaints_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.DealId, x.Status })
            .HasDatabaseName("IX_Complaints_DealId_Status");

        builder.HasIndex(x => new { x.RequestId, x.Status })
            .HasDatabaseName("IX_Complaints_RequestId_Status");
    }
}
