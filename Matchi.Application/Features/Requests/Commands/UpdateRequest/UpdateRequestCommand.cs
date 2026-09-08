using MediatR;

namespace Matchi.Application.Features.Requests.Commands.UpdateRequest;

public sealed record UpdateRequestCommand(
    long RequestId,
    CreateRequest.CreateRequestCommand Body) : IRequest<bool>;
