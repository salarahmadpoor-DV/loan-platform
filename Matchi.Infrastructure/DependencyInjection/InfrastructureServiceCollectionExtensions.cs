using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Matchi.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Matchi.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<MatchiDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });


        services.AddScoped<IBankRepository, BankRepository>();
        services.AddScoped<ILoanRequestRepository, LoanRequestRepository>();

        return services;
    }
}