using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record CreateBusinessCommand(string Name, string? Address, double? Lat, double? Lng, string? OwnerContact) : IRequest<long>;
