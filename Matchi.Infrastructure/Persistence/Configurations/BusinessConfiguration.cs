using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses", t =>
        {
            t.HasCheckConstraint("CK_Businesses_Rating", "[Rating]>=(0) AND [Rating]<=(5)");
        });

        builder.HasKey(x => x.Id).HasName("PK_Businesses");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Mobile)
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        builder.Property(x => x.Province)
            .HasMaxLength(100);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.District)
            .HasMaxLength(100);

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

        builder.HasOne(x => x.LogoMedia)
            .WithMany(x => x.LogoBusinesses)
            .HasForeignKey(x => x.LogoMediaId)
            .HasConstraintName("FK_Businesses_LogoMedia")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.OwnerUser)
            .WithMany(x => x.Businesses)
            .HasForeignKey(x => x.OwnerUserId)
            .HasConstraintName("FK_Businesses_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.WithoutSingleColumnIndex(nameof(Business.LogoMediaId));
        builder.WithoutSingleColumnIndex(nameof(Business.OwnerUserId));
        builder.HasIndex(x => x.OwnerUserId)
            .HasDatabaseName("IX_Businesses_OwnerUserId");
    }
}
