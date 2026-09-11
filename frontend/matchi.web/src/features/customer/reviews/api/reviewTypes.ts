/** Live GET /api/providers/{id}/reviews and GET /api/businesses/{id}/reviews (`ReviewDto`). */
export type ReviewDto = {
  id: number;
  targetId: number;
  targetType: string;
  rating: number;
  comment: string | null;
};

export type ReviewTargetKind = "Business" | "Provider";

export type ReviewTarget = {
  kind: ReviewTargetKind;
  id: number;
};

/** Live POST /api/deals/{dealId}/reviews (`CreateReviewBody`). XOR: one of businessId / providerId. */
export type CreateReviewBody = {
  rating: number;
  comment?: string | null;
  businessId?: number | null;
  providerId?: number | null;
};

export type CreateReviewResult = {
  reviewId: number;
};
