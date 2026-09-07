from pathlib import Path
CFG = Path(r"E:\Armin\Matchi\Matchi-platform\Matchi.Infrastructure\Persistence\Configurations")
H = """using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;
"""

def w(name, body):
    (CFG / name).write_text(H + body.replace("\r\n", "\n"), encoding="utf-8")

w("UserConfiguration.cs", """
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.ConfigureIdentity("PK_Users");
        builder.ConfigureAuditable();

        builder.Property(x => x.Mobile).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Name).HasMaxLength(200);
        builder.Property(x => x.IsMobileVerified).IsRequired().HasDefaultValue(false);

        builder.HasIndex(x => x.Mobile)
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_Users_Mobile");
    }
}
""")

w("CustomerConfiguration.cs", """
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.ConfigureIdentity("PK_Customers");
        builder.ConfigureAuditable();

        builder.Property(x => x.UserId).IsRequired();

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
""")

w("RoleConfiguration.cs", """
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.ConfigureIdentity("PK_Roles");
        builder.ConfigureTimestamped();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Roles_Code");
        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Roles_Name");
    }
}
""")

w("PermissionConfiguration.cs", """
public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.ConfigureIdentity("PK_Permissions");
        builder.ConfigureTimestamped();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasIndex(x => x.Code).IsUnique().HasDatabaseName("UX_Permissions_Code");
        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UX_Permissions_Name");
    }
}
""")

w("UserRoleConfiguration.cs", """
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(x => new { x.UserId, x.RoleId }).HasName("PK_UserRoles");
        builder.Property(x => x.CreateDate).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_UserRoles_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("FK_UserRoles_Roles")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
""")

w("RolePermissionConfiguration.cs", """
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(x => new { x.RoleId, x.PermissionId }).HasName("PK_RolePermissions");
        builder.Property(x => x.CreateDate).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("FK_RolePermissions_Roles")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .HasConstraintName("FK_RolePermissions_Permissions")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
""")

w("ProviderConfiguration.cs", """
public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("Providers", t =>
        {
            t.HasCheckConstraint("CK_Providers_Rating", "[Rating]>=(0) AND [Rating]<=(5)");
        });

        builder.ConfigureIdentity("PK_Providers");
        builder.ConfigureAuditable();

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Mobile).IsRequired().HasMaxLength(20);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Lat).HasPrecision(9, 6);
        builder.Property(x => x.Lng).HasPrecision(9, 6);
        builder.Property(x => x.Rating).HasPrecision(3, 2).IsRequired().HasDefaultValue(0m);
        builder.Property(x => x.ReviewCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.CompletedJobCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Active");

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
""")

w("BusinessConfiguration.cs", """
public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.ToTable("Businesses", t =>
        {
            t.HasCheckConstraint("CK_Businesses_Rating", "[Rating]>=(0) AND [Rating]<=(5)");
        });

        builder.ConfigureIdentity("PK_Businesses");
        builder.ConfigureAuditable();

        builder.Property(x => x.OwnerUserId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Mobile).HasMaxLength(20);
        builder.Property(x => x.Address).HasMaxLength(1000);
        builder.Property(x => x.Province).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.District).HasMaxLength(100);
        builder.Property(x => x.Lat).HasPrecision(9, 6);
        builder.Property(x => x.Lng).HasPrecision(9, 6);
        builder.Property(x => x.Rating).HasPrecision(3, 2).IsRequired().HasDefaultValue(0m);
        builder.Property(x => x.ReviewCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.CompletedJobCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Active");

        builder.HasOne(x => x.OwnerUser)
            .WithMany(x => x.Businesses)
            .HasForeignKey(x => x.OwnerUserId)
            .HasConstraintName("FK_Businesses_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LogoMedia)
            .WithMany(x => x.LogoBusinesses)
            .HasForeignKey(x => x.LogoMediaId)
            .HasConstraintName("FK_Businesses_LogoMedia")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.OwnerUserId).HasDatabaseName("IX_Businesses_OwnerUserId");
    }
}
""")

w("BusinessProviderConfiguration.cs", """
public class BusinessProviderConfiguration : IEntityTypeConfiguration<BusinessProvider>
{
    public void Configure(EntityTypeBuilder<BusinessProvider> builder)
    {
        builder.ToTable("BusinessProviders");
        builder.ConfigureIdentity("PK_BusinessProviders");
        builder.ConfigureAuditable();

        builder.Property(x => x.Role).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Status).IsRequired().HasMaxLength(30).HasDefaultValue("Active");
        builder.Property(x => x.JoinedAt).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");

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

        builder.HasIndex(x => x.BusinessId).HasDatabaseName("IX_BusinessProviders_BusinessId");
        builder.HasIndex(x => x.ProviderId).HasDatabaseName("IX_BusinessProviders_ProviderId");
        builder.HasIndex(x => new { x.BusinessId, x.ProviderId })
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))")
            .HasDatabaseName("UX_BusinessProviders_Active");
    }
}
""")

w("MediaConfiguration.cs", """
public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Media", t =>
        {
            t.HasCheckConstraint("CK_Media_Size", "[Size]>=(0)");
        });
        builder.ConfigureIdentity("PK_Media");
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(500);
        builder.Property(x => x.StorageKey).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Size).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(2000);
        builder.Property(x => x.CreateDate).IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
        builder.HasIndex(x => x.StorageKey).IsUnique().HasDatabaseName("UX_Media_StorageKey");
    }
}
""")

print("batch1 configs written")
