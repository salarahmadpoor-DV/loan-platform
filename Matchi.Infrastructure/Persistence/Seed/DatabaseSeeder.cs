using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MatchiDbContext context)
    {
        var requestViewPermission =
            await context.Permissions
                .FirstOrDefaultAsync(x => x.Code == "REQUEST_VIEW");

        if (requestViewPermission is null)
        {
            requestViewPermission = new Permission
            {
                Name = "مشاهده درخواست‌ها",
                Code = "REQUEST_VIEW",
                Description = "اجازه مشاهده درخواست‌ها",
                IsActive = true
            };

            await context.Permissions.AddAsync(requestViewPermission);
            await context.SaveChangesAsync();
        }

        var adminRole =
            await context.Roles
                .FirstOrDefaultAsync(x => x.Code == "ADMIN");

        if (adminRole is null)
        {
            adminRole = new Role
            {
                Name = "مدیر سیستم",
                Code = "ADMIN",
                Description = "مدیر سیستم با دسترسی کامل",
                IsActive = true
            };

            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        var rolePermissionExists =
            await context.RolePermissions
                .AnyAsync(x =>
                    x.RoleId == adminRole.Id &&
                    x.PermissionId == requestViewPermission.Id);

        if (!rolePermissionExists)
        {
            await context.RolePermissions.AddAsync(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = requestViewPermission.Id
            });

            await context.SaveChangesAsync();
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
        var financial = new ServiceCategory("وام", "loan");

        await context.ServiceCategories.AddRangeAsync(plumbing, financial);
        await context.SaveChangesAsync();

        var plumbingService = new Service("رفع نشتی و لوله‌کشی", plumbing.Id, "plumbing-repair");
        var loanService = new Service("وام خرد", financial.Id, "micro-loan");

        await context.Services.AddRangeAsync(plumbingService, loanService);
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
}
