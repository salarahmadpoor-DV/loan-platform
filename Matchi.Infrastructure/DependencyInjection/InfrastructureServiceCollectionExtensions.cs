using Matchi.Application.Common.Interfaces;
using Matchi.Domain.Interfaces;
using Matchi.Infrastructure.Persistence;
using Matchi.Infrastructure.Persistence.Repositories;
using Matchi.Infrastructure.Services;
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

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IOtpService, InMemoryOtpService>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<ILoanRequestRepository, LoanRequestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IIntroductionRepository, IntroductionRepository>();

        // new repositories for MVP
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IProviderRepository, ProviderRepository>();
        services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

        return services;
    }
}