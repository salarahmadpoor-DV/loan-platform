using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(x => x.Id).HasName("PK_Customers");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.ConfigureAuditable();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne(x => x.Customer)
            .HasForeignKey<Customer>(x => x.UserId)
            .HasConstraintName("FK_Customers_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_Customers_UserId");
    }
}
