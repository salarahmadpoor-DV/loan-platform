using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests", t =>
        {
            t.HasCheckConstraint("CK_Requests_RequestType", "[RequestType]=N'Hybrid' OR [RequestType]=N'Service' OR [RequestType]=N'Product'");
        });

        builder.HasKey(x => x.Id).HasName("PK_Requests");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.RequestType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Open");

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Requests)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Requests_Customers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.CustomerId, x.Status, x.CreateDate })
            .HasDatabaseName("IX_Requests_CustomerId_Status");
    }
}
