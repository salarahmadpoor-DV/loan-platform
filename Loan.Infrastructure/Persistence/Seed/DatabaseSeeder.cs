using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(LoanDbContext context)
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