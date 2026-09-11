import { getJson, postJson } from "../../../../shared/api/httpClient";
import type { CreateReviewBody, CreateReviewResult, ReviewDto } from "./reviewTypes";

export function getProviderReviews(providerId: number): Promise<ReviewDto[]> {
  return getJson<ReviewDto[]>(`/api/providers/${providerId}/reviews`);
}

export function getBusinessReviews(businessId: number): Promise<ReviewDto[]> {
  return getJson<ReviewDto[]>(`/api/businesses/${businessId}/reviews`);
}

export function createDealReview(
  dealId: number,
  body: CreateReviewBody,
): Promise<CreateReviewResult> {
  return postJson<CreateReviewResult, CreateReviewBody>(`/api/deals/${dealId}/reviews`, body);
}
