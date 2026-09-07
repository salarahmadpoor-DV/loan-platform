using Matchi.Domain.Interfaces;
using MediatR;

namespace Matchi.Application.Features.Requests.Commands;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, long>
{
    private readonly IRequestRepository _requestRepository;

    public CreateServiceRequestCommandHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<long> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var answers = (request.Answers ?? Enumerable.Empty<RequestAnswerDto>())
            .Select(a => (a.QuestionId, a.Text));

        return await _requestRepository.CreateServiceRequestAsync(
            request.UserId,
            request.ServiceId,
            request.Title,
            request.Description,
            request.Lat.HasValue ? (decimal)request.Lat.Value : null,
            request.Lng.HasValue ? (decimal)request.Lng.Value : null,
            answers,
            cancellationToken);
    }
}
