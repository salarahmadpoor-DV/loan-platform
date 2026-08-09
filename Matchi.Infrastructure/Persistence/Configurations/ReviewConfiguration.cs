using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.TargetType).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Rating).IsRequired();
        builder.HasOne(r => r.Introduction).WithMany().HasForeignKey(r => r.IntroductionId);
    }
}