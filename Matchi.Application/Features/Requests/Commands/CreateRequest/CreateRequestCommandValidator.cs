using FluentValidation;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests.Commands.CreateRequest;

public sealed class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator(IRequestRepository requests)
    {
        RequestWriteRules.Apply(this, requests);
    }
}
