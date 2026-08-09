using MediatR;

namespace Matchi.Application.Features.Users.Commands;

public sealed record UpdateUserProfileCommand(long UserId, string? Name, double? Lat, double? Lng, string? Address) : IRequest<bool>;
