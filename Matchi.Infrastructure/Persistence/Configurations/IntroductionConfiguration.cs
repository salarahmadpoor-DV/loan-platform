using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class IntroductionConfiguration : IEntityTypeConfiguration<Introduction>
{
    public void Configure(EntityTypeBuilder<Introduction> builder)
    {
        builder.ToTable("Introductions");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.TargetType).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Status).IsRequired().HasMaxLength(50);
        builder.HasOne(i => i.ServiceRequest).WithMany(r => r.Introductions).HasForeignKey(i => i.ServiceRequestId);
    }
}