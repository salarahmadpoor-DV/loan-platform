import { Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import type { ReviewDto } from "../api/reviewTypes";

type ReviewCardProps = {
  review: ReviewDto;
};

export function ReviewCard({ review }: ReviewCardProps) {
  return (
    <AppCard>
      <Stack spacing={0.5}>
        <Typography variant="body2">
          {t("review.card.rating", { rating: review.rating })}
        </Typography>
        {review.comment ? (
          <Typography variant="body2">{review.comment}</Typography>
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("review.card.noComment")}
          </Typography>
        )}
        <Typography variant="caption" color="text.secondary">
          {review.targetType === "Business"
            ? t("review.target.business", { id: review.targetId })
            : t("review.target.provider", { id: review.targetId })}
        </Typography>
      </Stack>
    </AppCard>
  );
}
