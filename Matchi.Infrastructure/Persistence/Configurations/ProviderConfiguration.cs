using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("Providers", t =>
        {
            t.HasCheckConstraint("CK_Providers_Rating", "[Rating]>=(0) AND [Rating]<=(5)");
        });

        builder.HasKey(x => x.Id).HasName("PK_Providers");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Mobile)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Lat)
            .HasPrecision(9, 6);

        builder.Property(x => x.Lng)
            .HasPrecision(9, 6);

        builder.Property(x => x.Rating)
            .IsRequired()
            .HasPrecision(3, 2)
            .HasDefaultValue(0m);

        builder.Property(x => x.ReviewCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CompletedJobCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("Active");

        builder.HasOne(x => x.User)
            .WithOne(x => x.Provider)
            .HasForeignKey<Provider>(x => x.UserId)
            .HasConstraintName("FK_Providers_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_Providers_UserId");
    }
}
