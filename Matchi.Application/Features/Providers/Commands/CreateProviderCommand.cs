using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record CreateProviderCommand(string Name, string Mobile, double? Lat, double? Lng) : IRequest<long>;
