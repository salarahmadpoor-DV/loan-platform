using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CancelRequest;

public sealed record CancelRequestCommand(long RequestId) : IRequest<bool>;
