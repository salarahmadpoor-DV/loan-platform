using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MatchiDbContext context)
    {
        if (!await context.Banks.AnyAsync())
        {
            var banks = new List<Bank>
            {
                new Bank(
                    "بانک قرض الحسنه رسالت",
                    "resalat")
                ,

                new Bank(
                    "بانک ملت",
                    "mellat")
                ,

                new Bank(
                    "بانک ملی ایران",
                    "melli")
            };

            await context.Banks.AddRangeAsync(banks);

            await context.SaveChangesAsync();
        }

        // Seed a demo user if none exists (Mobile used as primary identifier)
        if (!await context.Set<User>().AnyAsync())
        {
            var demoUser = new User("09120000000");
            demoUser.UpdateProfile("Demo User");
            await context.Set<User>().AddAsync(demoUser);
            await context.SaveChangesAsync();
        }

        // Seed service categories, services and a few providers/businesses for Karaj area
        if (!await context.Set<ServiceCategory>().AnyAsync())
        {
            var plumbing = new ServiceCategory("لوله‌کشی", "plumbing");
            var financial = new ServiceCategory("وام", "loan");
            await context.Set<ServiceCategory>().AddRangeAsync(plumbing, financial);
            await context.SaveChangesAsync();

            var plumbingService = new Service("رفع نشتی و لوله‌کشی", plumbing.Id);
            var loanService = new Service("وام خرد", financial.Id);
            await context.Set<Service>().AddRangeAsync(plumbingService, loanService);
            await context.SaveChangesAsync();

            // Add sample providers in Karaj (lat/lng approximate)
            var providers = new List<Provider>
            {
                new Provider("علی رضایی", "09121110001", 35.832, 50.995),
                new Provider("مهدی حسینی", "09121110002", 35.843, 50.987),
                new Provider("رضا موسوی", "09121110003", 35.829, 50.998),
                new Provider("سارا احمدپور", "09121110004", 35.836, 50.990),
                new Provider("پویا کاظمی", "09121110005", 35.838, 50.999)
            };

            await context.Set<Provider>().AddRangeAsync(providers);
            await context.SaveChangesAsync();

            // assign plumbing service to providers
            var providerServices = providers.Select(p => new ProviderService(p.Id, plumbingService.Id)).ToList();
            await context.Set<ProviderService>().AddRangeAsync(providerServices);
            await context.SaveChangesAsync();

            // sample businesses
            var biz = new Business("تأسیسات البرز", "کرج، فلان خیابان", 35.835, 50.994);
            await context.Set<Business>().AddAsync(biz);
            await context.SaveChangesAsync();

            var bp = new BusinessProvider(biz.Id, providers[0].Id, "Manager");
            bp = new BusinessProvider(biz.Id, providers[1].Id, "Technician");
            await context.Set<BusinessProvider>().AddAsync(bp);
            await context.SaveChangesAsync();
        }
    }
}