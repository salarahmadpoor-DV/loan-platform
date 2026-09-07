using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessProviderConfiguration : IEntityTypeConfiguration<BusinessProvider>
{
    public void Configure(EntityTypeBuilder<BusinessProvider> builder)
    {
        builder.ToTable("BusinessProviders");

        builder.HasKey(x => x.Id).HasName("PK_BusinessProviders");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.BusinessId)
            .IsRequired();

        builder.Property(x => x.ProviderId)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Active");

        builder.Property(x => x.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");


        builder.HasOne(x => x.Business)
            .WithMany(x => x.BusinessProviders)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_BusinessProviders_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.BusinessProviders)
            .HasForeignKey(x => x.ProviderId)
            .HasConstraintName("FK_BusinessProviders_Providers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.BusinessId)
            .HasDatabaseName("IX_BusinessProviders_BusinessId");

        builder.HasIndex(x => x.ProviderId)
            .HasDatabaseName("IX_BusinessProviders_ProviderId");

        builder.HasIndex(x => new { x.BusinessId, x.ProviderId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_BusinessProviders_Active");
    }
}
