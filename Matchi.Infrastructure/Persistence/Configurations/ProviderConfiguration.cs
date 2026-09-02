using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("Providers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Mobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Rating)
            .HasDefaultValue(0);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Provider)
            .HasForeignKey<Provider>(x => x.UserId)
            .HasConstraintName("FK_Providers_Users")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL")
            .HasDatabaseName("UQ_Providers_UserId");
    }
}