using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MatchiDbContext context)
    {
        var userRole = await EnsureRoleAsync(context, "USER", "کاربر", "Authenticated marketplace user");
        var adminRole = await EnsureRoleAsync(context, "ADMIN", "مدیر سیستم", "مدیر سیستم با دسترسی کامل");

        var selfService = new[]
        {
            ("PROVIDER_VIEW", "مشاهده ارائه‌دهنده"),
            ("PROVIDER_CREATE", "ایجاد ارائه‌دهنده"),
            ("PROVIDER_EDIT", "ویرایش ارائه‌دهنده"),
            ("BUSINESS_VIEW", "مشاهده کسب‌وکار"),
            ("BUSINESS_CREATE", "ایجاد کسب‌وکار"),
            ("BUSINESS_EDIT", "ویرایش کسب‌وکار")
        };

        var catalogAdmin = new[]
        {
            ("SERVICE_VIEW", "مشاهده خدمت"),
            ("SERVICE_CREATE", "ایجاد خدمت"),
            ("SERVICE_EDIT", "ویرایش خدمت"),
            ("PRODUCT_VIEW", "مشاهده محصول"),
            ("PRODUCT_CREATE", "ایجاد محصول"),
            ("PRODUCT_EDIT", "ویرایش محصول"),
            ("REQUEST_VIEW", "مشاهده درخواست‌ها")
        };

        foreach (var (code, name) in selfService.Concat(catalogAdmin))
        {
            var permission = await EnsurePermissionAsync(context, code, name);
            await EnsureRolePermissionAsync(context, adminRole.Id, permission.Id);
            if (selfService.Any(x => x.Item1 == code))
                await EnsureRolePermissionAsync(context, userRole.Id, permission.Id);
        }

        var demoUser =
            await context.Users
                .FirstOrDefaultAsync(x => x.Mobile == "09120000000");

        if (demoUser is null)
        {
            demoUser = new User("09120000000");
            demoUser.UpdateProfile("Demo User");
            demoUser.VerifyMobile();

            await context.Users.AddAsync(demoUser);
            await context.SaveChangesAsync();
        }

        var userRoleExists =
            await context.UserRoles
                .AnyAsync(x =>
                    x.UserId == demoUser.Id &&
                    x.RoleId == adminRole.Id);

        if (!userRoleExists)
        {
            await context.UserRoles.AddAsync(new UserRole
            {
                UserId = demoUser.Id,
                RoleId = adminRole.Id,
                CreateDate = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        if (await context.ServiceCategories.AnyAsync())
            return;

        var plumbing = new ServiceCategory("لوله‌کشی", "plumbing");

        await context.ServiceCategories.AddAsync(plumbing);
        await context.SaveChangesAsync();

        var plumbingService = new Service("رفع نشتی و لوله‌کشی", plumbing.Id, "plumbing-repair");

        await context.Services.AddAsync(plumbingService);
        await context.SaveChangesAsync();

        var providerSpecs = new (string Name, string Mobile, decimal Lat, decimal Lng)[]
        {
            ("علی رضایی", "09121110001", 35.832m, 50.995m),
            ("مهدی حسینی", "09121110002", 35.843m, 50.987m),
            ("رضا موسوی", "09121110003", 35.829m, 50.998m),
            ("سارا احمدپور", "09121110004", 35.836m, 50.990m),
            ("پویا کاظمی", "09121110005", 35.838m, 50.999m)
        };

        var providers = new List<Provider>();
        foreach (var spec in providerSpecs)
        {
            var user = new User(spec.Mobile);
            user.UpdateProfile(spec.Name);
            user.VerifyMobile();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            providers.Add(new Provider(user.Id, spec.Name, spec.Mobile, spec.Lat, spec.Lng));
        }

        await context.Providers.AddRangeAsync(providers);
        await context.SaveChangesAsync();

        await context.ProviderServices.AddRangeAsync(
            providers.Select(p => new ProviderService(p.Id, plumbingService.Id)).ToList());
        await context.SaveChangesAsync();

        var business = new Business(
            demoUser.Id,
            "تأسیسات البرز",
            "کرج، فلان خیابان",
            35.835m,
            50.994m);

        await context.Businesses.AddAsync(business);
        await context.SaveChangesAsync();

        await context.BusinessProviders.AddRangeAsync(
            new BusinessProvider(business.Id, providers[0].Id, "Manager"),
            new BusinessProvider(business.Id, providers[1].Id, "Technician"));
        await context.SaveChangesAsync();
    }

    private static async Task<Role> EnsureRoleAsync(
        MatchiDbContext context,
        string code,
        string name,
        string description)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Code == code);
        if (role is not null)
            return role;

        role = new Role
        {
            Name = name,
            Code = code,
            Description = description,
            IsActive = true
        };
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
        return role;
    }

    private static async Task<Permission> EnsurePermissionAsync(
        MatchiDbContext context,
        string code,
        string name)
    {
        var permission = await context.Permissions.FirstOrDefaultAsync(x => x.Code == code);
        if (permission is not null)
            return permission;

        permission = new Permission
        {
            Name = name,
            Code = code,
            Description = name,
            IsActive = true
        };
        await context.Permissions.AddAsync(permission);
        await context.SaveChangesAsync();
        return permission;
    }

    private static async Task EnsureRolePermissionAsync(
        MatchiDbContext context,
        long roleId,
        long permissionId)
    {
        var exists = await context.RolePermissions.AnyAsync(
            x => x.RoleId == roleId && x.PermissionId == permissionId);
        if (exists)
            return;

        await context.RolePermissions.AddAsync(new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreateDate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }
}
