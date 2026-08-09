
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Matchi.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(ApplicationServiceCollectionExtensions).Assembly);
        });

        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.MediatR.ValidationBehavior<,>));

        return services;
    }
}
