import { useQuery } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import { getBusinessReviews } from "../api/reviewsApi";

export function useBusinessReviews(businessId: number | undefined) {
  return useQuery({
    queryKey:
      businessId != null
        ? queryKeys.reviews.byBusiness(businessId)
        : queryKeys.reviews.all,
    queryFn: () => getBusinessReviews(businessId as number),
    enabled: businessId != null && Number.isFinite(businessId) && businessId > 0,
  });
}
