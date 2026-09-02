 
using MediatR;

namespace Matchi.Application.Features.Providers.Commands;

public sealed record CreateProviderCommand(
    string Name,
    double? Lat,
    double? Lng) : IRequest<long>;
 
