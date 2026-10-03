using FluentValidation;
using Matchi.Application.Common.Interfaces;
using Matchi.Application.Notifications;
using Matchi.Application.Workspaces;
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

        services.AddValidatorsFromAssembly(
            typeof(ApplicationServiceCollectionExtensions).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Common.MediatR.ValidationBehavior<,>));
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<NotificationRecipientResolver>();
        services.AddScoped<IWorkspaceAccessService, WorkspaceAccessService>();

        return services;
    }
}
