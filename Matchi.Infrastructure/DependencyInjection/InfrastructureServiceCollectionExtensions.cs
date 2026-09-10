using Matchi.Application.Common.Interfaces;
using Matchi.Application.Common.Models;
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
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IOtpService>(sp =>
        {
            var otpOptions = new OtpOptions
            {
                TtlMinutes = ParsePositiveInt(configuration["Otp:TtlMinutes"], 5),
                MaxAttempts = ParsePositiveInt(configuration["Otp:MaxAttempts"], 5)
            };
            return new InMemoryOtpService(sp.GetRequiredService<TimeProvider>(), otpOptions);
        });
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IProviderRepository, ProviderRepository>();
        services.AddScoped<IBusinessRepository, BusinessRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IProposalRepository, ProposalRepository>();
        services.AddScoped<IDealRepository, DealRepository>();
        services.AddScoped<IServiceExecutionRepository, ServiceExecutionRepository>();
        services.AddScoped<IExecutionAssignmentRepository, ExecutionAssignmentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IMatchingReadRepository, MatchingReadRepository>();

        return services;
    }

    private static int ParsePositiveInt(string? value, int fallback)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : fallback;
    }
}
