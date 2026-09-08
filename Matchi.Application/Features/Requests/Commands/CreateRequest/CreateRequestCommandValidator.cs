using FluentValidation;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator(IRequestRepository requests)
    {
        RequestWriteRules.Apply(
            this,
            x => x.RequestType,
            x => x.Title,
            x => x.Services,
            x => x.Products,
            x => x.Location,
            x => x.Schedule,
            requests);
    }
}
