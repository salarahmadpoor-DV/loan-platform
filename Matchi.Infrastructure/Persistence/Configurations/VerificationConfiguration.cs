using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
{
    public void Configure(EntityTypeBuilder<Verification> builder)
    {
        builder.ToTable("Verifications");

        builder.HasKey(x => x.Id).HasName("PK_Verifications");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureTimestamped();

        builder.Property(x => x.EntityType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.EntityId)
            .IsRequired();

        builder.Property(x => x.VerificationType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Provider)
            .HasMaxLength(100);

        builder.Property(x => x.Reference)
            .HasMaxLength(200);



        builder.Property(x => x.RejectReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => new { x.EntityType, x.EntityId, x.Status })
            .HasDatabaseName("IX_Verifications_Entity");
    }
}
