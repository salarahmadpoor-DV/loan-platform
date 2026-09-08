using MediatR;

namespace Matchi.Application.Features.Requests.Commands.DeleteRequest;

public sealed record DeleteRequestCommand(long RequestId) : IRequest<bool>;
