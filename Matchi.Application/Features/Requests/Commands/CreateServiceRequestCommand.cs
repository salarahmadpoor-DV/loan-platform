using MediatR;
using System.Collections.Generic;

namespace Matchi.Application.Features.Requests.Commands;

public record CreateServiceRequestCommand(long UserId, long ServiceId, string Title, string? Description, double? Lat, double? Lng, IEnumerable<RequestAnswerDto>? Answers) : IRequest<long>;

public record RequestAnswerDto(long QuestionId, long? OptionId, string? Text);
