using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media", t =>
        {
            t.HasCheckConstraint("CK_Media_Size", "[Size]>=(0)");
        });

        builder.HasKey(x => x.Id).HasName("PK_Media");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StorageKey)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(2000);

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasIndex(x => x.StorageKey)
            .IsUnique()
            .HasDatabaseName("UX_Media_StorageKey");
    }
}
