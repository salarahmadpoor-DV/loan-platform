import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getProviderReviews } from "../api/reviewsApi";

export function useProviderReviews(providerId: number | undefined) {
  return useQuery({
    queryKey:
      providerId != null
        ? queryKeys.reviews.byProvider(providerId)
        : queryKeys.reviews.all,
    queryFn: () => getProviderReviews(providerId as number),
    enabled: providerId != null && Number.isFinite(providerId) && providerId > 0,
  });
}
