import { useMutation, useQueryClient } from "@tanstack/react-query";
import { queryKeys } from "../../../../shared/api/queryKeys";
import type { CreateReviewBody } from "../api/reviewTypes";
import { createDealReview } from "../api/reviewsApi";

export function useCreateReview(dealId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateReviewBody) => createDealReview(dealId, body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.reviews.all });
    },
  });
}
