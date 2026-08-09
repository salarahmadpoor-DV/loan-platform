using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Mobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(u => u.Mobile)
            .IsUnique();

        builder.Property(u => u.Name)
            .HasMaxLength(200);

        builder.Property(u => u.IsMobileVerified)
            .HasDefaultValue(false);
    }
}
