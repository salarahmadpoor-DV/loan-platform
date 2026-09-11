using Matchi.Domain.Entities;

namespace Matchi.Application.Features.Providers;

/// <summary>
/// Provider marketplace visibility. Membership (<c>BusinessProvider</c>) is not a grant.
/// </summary>
internal static class ProviderMarketplaceVisibility
{
    public static bool IsActiveAssignment(ExecutionAssignment assignment) =>
        assignment.ProviderId > 0
        && string.Equals(assignment.Status, "Assigned", StringComparison.Ordinal);

    public static bool CanSeeDeal(Deal deal, long providerId)
    {
        if (deal.IsDeleted)
            return false;

        if (deal.Proposal.ProviderId == providerId)
            return true;

        return deal.ServiceExecutions.Any(execution =>
            execution.Assignments.Any(assignment =>
                assignment.ProviderId == providerId && IsActiveAssignment(assignment)));
    }

    public static bool CanSeeExecution(ServiceExecution execution, long providerId)
    {
        if (execution.Deal.Proposal.ProviderId == providerId)
            return true;

        return execution.Assignments.Any(assignment =>
            assignment.ProviderId == providerId && IsActiveAssignment(assignment));
    }
}
