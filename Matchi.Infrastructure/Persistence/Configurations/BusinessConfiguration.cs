using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(x => x.OwnerUser)
            .WithMany(x => x.Businesses)
            .HasForeignKey(x => x.OwnerUserId)
            .HasConstraintName("FK_Businesses_Users_OwnerUserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}