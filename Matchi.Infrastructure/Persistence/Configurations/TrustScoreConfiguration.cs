using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class TrustScoreConfiguration : IEntityTypeConfiguration<TrustScore>
{
    public void Configure(EntityTypeBuilder<TrustScore> builder)
    {
        builder.ToTable("TrustScores", t =>
        {
            t.HasCheckConstraint("CK_TrustScores_Score", "[Score]>=(0) AND [Score]<=(100)");
        });

        builder.HasKey(x => x.Id).HasName("PK_TrustScores");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.EntityId)
            .IsRequired();

        builder.Property(x => x.Score)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(x => x.CalculatedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.CalculatedAt })
            .HasDatabaseName("IX_TrustScores_Entity");
    }
}
