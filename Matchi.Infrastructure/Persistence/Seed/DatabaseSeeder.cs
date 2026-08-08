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
    }
}