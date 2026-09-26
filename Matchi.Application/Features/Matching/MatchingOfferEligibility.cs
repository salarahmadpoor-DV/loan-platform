namespace Matchi.Application.Features.Matching;

/// <summary>
/// Catalog eligibility used by matching. A candidate matches a requested service/product
/// if they offer <em>any</em> of the requested ids (not all lines). Hybrid requests
/// require a service match; product is a score bonus unless the request is product-only.
/// </summary>
internal static class MatchingOfferEligibility
{
    public static bool ServiceLinkMatches(
        bool isDeleted,
        bool isActive,
        long offeredServiceId,
        IReadOnlyCollection<long> requestedServiceIds)
    {
        return !isDeleted
               && isActive
               && requestedServiceIds.Contains(offeredServiceId);
    }

    public static bool ProductLinkMatches(
        bool isDeleted,
        bool isAvailable,
        long offeredProductId,
        long offeredProductCategoryId,
        bool catalogProductDeleted,
        IReadOnlyCollection<long> requestedProductIds,
        IReadOnlyCollection<long> requestedCategoryIds)
    {
        if (isDeleted || !isAvailable || catalogProductDeleted)
            return false;

        return requestedProductIds.Contains(offeredProductId)
               || requestedCategoryIds.Contains(offeredProductCategoryId);
    }

    public static bool CandidateQualifies(
        bool requireService,
        bool serviceMatch,
        bool requireProduct,
        bool productMatch)
    {
        return (!requireService || serviceMatch) && (!requireProduct || productMatch);
    }
}
