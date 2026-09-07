using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id).HasName("PK_Users");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.Mobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.IsMobileVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(x => x.Mobile)
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_Users_Mobile");
    }
}
