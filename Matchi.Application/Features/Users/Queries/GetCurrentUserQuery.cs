using MediatR;

namespace Matchi.Application.Features.Users.Queries;

public sealed record GetCurrentUserQuery(long UserId) : IRequest<UserDto?>;
