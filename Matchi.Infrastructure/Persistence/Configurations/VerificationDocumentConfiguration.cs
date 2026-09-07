using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class VerificationDocumentConfiguration : IEntityTypeConfiguration<VerificationDocument>
{
    public void Configure(EntityTypeBuilder<VerificationDocument> builder)
    {
        builder.ToTable("VerificationDocuments");

        builder.HasKey(x => x.Id).HasName("PK_VerificationDocuments");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.VerificationId)
            .IsRequired();

        builder.Property(x => x.MediaId)
            .IsRequired();

        builder.Property(x => x.DocumentType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.UploadedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");


        builder.Property(x => x.RejectReason)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Media)
            .WithMany(x => x.VerificationDocuments)
            .HasForeignKey(x => x.MediaId)
            .HasConstraintName("FK_VerificationDocuments_Media")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Verification)
            .WithMany(x => x.Documents)
            .HasForeignKey(x => x.VerificationId)
            .HasConstraintName("FK_VerificationDocuments_Verifications")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.VerificationId)
            .HasDatabaseName("IX_VerificationDocuments_VerificationId");
    }
}
