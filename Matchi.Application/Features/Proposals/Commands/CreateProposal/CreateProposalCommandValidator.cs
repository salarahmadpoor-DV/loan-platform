using FluentValidation;
using Matchi.Application.Features.Proposals;

namespace Matchi.Application.Features.Proposals.Commands.CreateProposal;

public sealed class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
{
    public CreateProposalCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.ProposerType)
            .Must(t => t is "Provider" or "Business")
            .WithMessage("ProposerType must be Provider or Business.");

        RuleFor(x => x.TotalPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DeliveryFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Message).MaximumLength(2000);

        RuleFor(x => x.ProposedTimeFrom)
            .Must((cmd, from) => from is null || cmd.ProposedTimeTo is null || from < cmd.ProposedTimeTo)
            .WithMessage("ProposedTimeFrom must be earlier than ProposedTimeTo.");

        When(x => x.ExpireAt.HasValue, () =>
        {
            RuleFor(x => x.ExpireAt)
                .Must(expireAt => expireAt > DateTime.UtcNow)
                .WithMessage("ExpireAt must be in the future.");
        });

        When(x => x.ProposerType == "Provider", () =>
        {
            RuleFor(x => x.BusinessId)
                .Null()
                .WithMessage("BusinessId is not used when proposing as a Provider.");
        });

        When(x => x.ProposerType == "Business" && x.BusinessId.HasValue, () =>
        {
            RuleFor(x => x.BusinessId!.Value).GreaterThan(0);
        });

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one proposal item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemType)
                .Must(t => t is "Product" or "Service")
                .WithMessage("ItemType must be Product or Service.");

            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
            item.RuleFor(i => i.TotalPrice).GreaterThanOrEqualTo(0);
            item.RuleFor(i => i.Description).MaximumLength(2000);

            item.When(i => i.ItemType == "Service", () =>
            {
                item.RuleFor(i => i.ServiceId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Service items require serviceId.");
                item.RuleFor(i => i.ProductId)
                    .Null()
                    .WithMessage("Service items cannot include productId.");
            });

            item.When(i => i.ItemType == "Product", () =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("Product items require productId.");
                item.RuleFor(i => i.ServiceId)
                    .Null()
                    .WithMessage("Product items cannot include serviceId.");
            });
        });
    }
}
