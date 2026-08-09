using MediatR;
using Matchi.Domain.Entities;
using Matchi.Domain.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Matchi.Application.Features.Requests.Commands;

public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, long>
{
    private readonly IServiceRequestRepository _requestRepository;

    public CreateServiceRequestCommandHandler(IServiceRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<long> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
    {
        var sr = new ServiceRequest(request.UserId, request.ServiceId, request.Title, request.Description, request.Lat, request.Lng);

        var answers = (request.Answers ?? Enumerable.Empty<RequestAnswerDto>()).Select(a => new RequestAnswer(0, a.QuestionId, a.OptionId, a.Text));

        var id = await _requestRepository.CreateRequestAsync(sr, answers, cancellationToken);

        return id;
    }
}