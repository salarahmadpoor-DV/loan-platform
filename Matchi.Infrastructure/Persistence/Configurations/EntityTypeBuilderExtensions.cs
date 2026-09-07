using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Matchi.Domain.Common;

namespace Matchi.Infrastructure.Persistence.Configurations;

internal static class EntityTypeBuilderExtensions
{
    public static void ConfigureIdentity<T>(this EntityTypeBuilder<T> builder, string pkName)
        where T : Entity
    {
        builder.HasKey(x => x.Id).HasName(pkName);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
    }

    public static void ConfigureCreateDate<T>(this EntityTypeBuilder<T> builder, string defaultName)
        where T : class
    {
        builder.Property("CreateDate")
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }

    public static void ConfigureTimestamped<T>(this EntityTypeBuilder<T> builder)
        where T : TimestampedEntity
    {
        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.UpdateDate);
    }

    public static void ConfigureAuditable<T>(this EntityTypeBuilder<T> builder)
        where T : AuditableEntity
    {
        builder.ConfigureTimestamped();
        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }

    public static void Restrict<T, TRelated>(
        this ReferenceCollectionBuilder<TRelated, T> builder,
        string constraintName)
        where T : class
        where TRelated : class
    {
        builder.HasConstraintName(constraintName).OnDelete(DeleteBehavior.NoAction);
    }
}
