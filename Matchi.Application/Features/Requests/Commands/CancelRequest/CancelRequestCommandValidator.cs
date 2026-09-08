using FluentValidation;

namespace Matchi.Application.Features.Requests.Commands.CancelRequest;

public sealed class CancelRequestCommandValidator : AbstractValidator<CancelRequestCommand>
{
    public CancelRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("Request id is required.");
    }
}
