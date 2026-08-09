using MediatR;

namespace Matchi.Application.Features.Businesses.Commands;

public sealed record InviteProviderCommand(long BusinessId, long ProviderId, string? Role) : IRequest<long>;
