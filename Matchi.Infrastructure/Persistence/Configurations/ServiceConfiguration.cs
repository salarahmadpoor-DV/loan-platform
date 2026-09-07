using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(x => x.Id).HasName("PK_Services");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Services)
            .HasForeignKey(x => x.CategoryId)
            .HasConstraintName("FK_Services_ServiceCategories")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("([Slug] IS NOT NULL AND [IsDeleted]=(0))")
            .HasDatabaseName("UX_Services_Slug");
    }
}
