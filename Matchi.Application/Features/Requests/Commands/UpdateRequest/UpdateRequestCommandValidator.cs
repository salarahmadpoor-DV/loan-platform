using FluentValidation;
using Matchi.Application.Features.Requests.Commands.CreateRequest;
using Matchi.Domain.Interfaces;

namespace Matchi.Application.Features.Requests.Commands.UpdateRequest;

public sealed class UpdateRequestCommandValidator : AbstractValidator<UpdateRequestCommand>
{
    public UpdateRequestCommandValidator(IRequestRepository requests)
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("Request id is required.");

        RuleFor(x => x.Body)
            .NotNull()
            .SetValidator(new CreateRequestCommandValidator(requests));
    }
}
