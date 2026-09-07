using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(x => x.Id).HasName("PK_Permissions");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("UX_Permissions_Code");

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("UX_Permissions_Name");
    }
}
