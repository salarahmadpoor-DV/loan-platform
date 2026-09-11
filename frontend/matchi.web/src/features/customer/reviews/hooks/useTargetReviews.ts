import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import type { ReviewTarget } from "../api/reviewTypes";
import { getBusinessReviews, getProviderReviews } from "../api/reviewsApi";

export function useTargetReviews(target: ReviewTarget | undefined) {
  const isBusiness = target?.kind === "Business";
  const isProvider = target?.kind === "Provider";

  return useQuery({
    queryKey: isBusiness
      ? queryKeys.reviews.byBusiness(target.id)
      : isProvider
        ? queryKeys.reviews.byProvider(target.id)
        : queryKeys.reviews.all,
    queryFn: () =>
      isBusiness
        ? getBusinessReviews(target!.id)
        : getProviderReviews(target!.id),
    enabled: target != null && Number.isFinite(target.id) && target.id > 0,
  });
}
