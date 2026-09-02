using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MatchiDbContext context)
    {
        // =========================================================
        // 1. Permissions
        // =========================================================

        var requestViewPermission =
            await context.Set<Permission>()
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

            await context.Set<Permission>()
                .AddAsync(requestViewPermission);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 2. Roles
        // =========================================================

        var adminRole =
            await context.Set<Role>()
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

            await context.Set<Role>()
                .AddAsync(adminRole);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 3. Role Permissions
        // =========================================================

        var rolePermissionExists =
            await context.Set<RolePermission>()
                .AnyAsync(x =>
                    x.RoleId == adminRole.Id &&
                    x.PermissionId == requestViewPermission.Id);

        if (!rolePermissionExists)
        {
            var rolePermission = new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = requestViewPermission.Id
            };

            await context.Set<RolePermission>()
                .AddAsync(rolePermission);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 4. Banks
        // =========================================================

        if (!await context.Banks.AnyAsync())
        {
            var banks = new List<Bank>
            {
                new Bank(
                    "بانک قرض الحسنه رسالت",
                    "resalat"),

                new Bank(
                    "بانک ملت",
                    "mellat"),

                new Bank(
                    "بانک ملی ایران",
                    "melli")
            };

            await context.Banks.AddRangeAsync(banks);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 5. Demo User
        // =========================================================

        var demoUser =
            await context.Set<User>()
                .FirstOrDefaultAsync(x => x.Mobile == "09120000000");

        if (demoUser is null)
        {
            demoUser = new User("09120000000");

            demoUser.UpdateProfile("Demo User");
            demoUser.VerifyMobile();

            await context.Set<User>()
                .AddAsync(demoUser);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 6. Assign ADMIN role to Demo User
        // =========================================================

        var userRoleExists =
            await context.Set<UserRole>()
                .AnyAsync(x =>
                    x.UserId == demoUser.Id &&
                    x.RoleId == adminRole.Id);

        if (!userRoleExists)
        {
            var userRole = new UserRole
            {
                UserId = demoUser.Id,
                RoleId = adminRole.Id,
                CreateDate = DateTime.UtcNow
            };

            await context.Set<UserRole>()
                .AddAsync(userRole);

            await context.SaveChangesAsync();
        }


        // =========================================================
        // 7. Service Categories / Services / Providers
        // =========================================================

        if (!await context.Set<ServiceCategory>().AnyAsync())
        {
            var plumbing =
                new ServiceCategory("لوله‌کشی", "plumbing");

            var financial =
                new ServiceCategory("وام", "loan");

            await context.Set<ServiceCategory>()
                .AddRangeAsync(plumbing, financial);

            await context.SaveChangesAsync();


            var plumbingService =
                new Service(
                    "رفع نشتی و لوله‌کشی",
                    plumbing.Id);

            var loanService =
                new Service(
                    "وام خرد",
                    financial.Id);

            await context.Set<Service>()
                .AddRangeAsync(
                    plumbingService,
                    loanService);

            await context.SaveChangesAsync();


            // Sample providers
            var providers = new List<Provider>
            {
                new Provider(
                    "علی رضایی",
                    "09121110001",
                    35.832,
                    50.995),

                new Provider(
                    "مهدی حسینی",
                    "09121110002",
                    35.843,
                    50.987),

                new Provider(
                    "رضا موسوی",
                    "09121110003",
                    35.829,
                    50.998),

                new Provider(
                    "سارا احمدپور",
                    "09121110004",
                    35.836,
                    50.990),

                new Provider(
                    "پویا کاظمی",
                    "09121110005",
                    35.838,
                    50.999)
            };

            await context.Set<Provider>()
                .AddRangeAsync(providers);

            await context.SaveChangesAsync();


            // Assign plumbing service to providers
            var providerServices =
                providers
                    .Select(p =>
                        new ProviderService(
                            p.Id,
                            plumbingService.Id))
                    .ToList();

            await context.Set<ProviderService>()
                .AddRangeAsync(providerServices);

            await context.SaveChangesAsync();


            // Sample business
            var biz =
                new Business(
                    "تأسیسات البرز",
                    "کرج، فلان خیابان",
                    35.835,
                    50.994);

            await context.Set<Business>()
                .AddAsync(biz);

            await context.SaveChangesAsync();


            // Assign both providers to business
            var businessProviders = new List<BusinessProvider>
            {
                new BusinessProvider(
                    biz.Id,
                    providers[0].Id,
                    "Manager"),

                new BusinessProvider(
                    biz.Id,
                    providers[1].Id,
                    "Technician")
            };

            await context.Set<BusinessProvider>()
                .AddRangeAsync(businessProviders);

            await context.SaveChangesAsync();
        }
    }
}

