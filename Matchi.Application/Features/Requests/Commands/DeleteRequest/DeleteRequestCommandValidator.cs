using FluentValidation;

namespace Matchi.Application.Features.Requests.Commands.DeleteRequest;

public sealed class DeleteRequestCommandValidator : AbstractValidator<DeleteRequestCommand>
{
    public DeleteRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .GreaterThan(0).WithMessage("Request id is required.");
    }
}
