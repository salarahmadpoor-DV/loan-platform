using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.ConfigureIdentity("PK_Notifications");

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.EntityType)
            .HasMaxLength(80);

        builder.Property(x => x.ActionUrl)
            .HasMaxLength(400);

        builder.Property(x => x.ReferenceKey)
            .HasMaxLength(160);

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasOne(x => x.User)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_Notifications_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("IX_Notifications_UserId");

        builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt })
            .HasDatabaseName("IX_Notifications_UserId_IsRead_CreatedAt");

        builder.HasIndex(x => new { x.UserId, x.ReferenceKey })
            .IsUnique()
            .HasFilter("[ReferenceKey] IS NOT NULL")
            .HasDatabaseName("UX_Notifications_UserId_ReferenceKey");
    }
}
