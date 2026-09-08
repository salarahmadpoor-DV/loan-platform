using MediatR;

namespace Matchi.Application.Features.Requests.Commands.CreateRequest;

public sealed record CreateRequestCommand(
    string RequestType,
    string Title,
    string? Description = null,
    IReadOnlyList<RequestServiceLineDto>? Services = null,
    IReadOnlyList<RequestProductLineDto>? Products = null,
    RequestLocationDto? Location = null,
    RequestScheduleDto? Schedule = null) : IRequest<long>;
