using FluentValidation;

namespace Matchi.Application.Features.Users.Commands;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Authenticated user id is required.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name must be at most 100 characters.");
    }
}
