import { Rating, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ReviewDto } from "../api/reviewTypes";

type ReviewCardProps = {
  review: ReviewDto;
};

export function ReviewCard({ review }: ReviewCardProps) {
  const targetLabel =
    review.targetType === "Business"
      ? t("review.target.business", { id: review.targetId })
      : t("review.target.provider", { id: review.targetId });

  return (
    <AppCard>
      <Stack spacing={1}>
        <Stack
          direction={{ xs: "column", sm: "row" }}
          justifyContent="space-between"
          alignItems={{ xs: "flex-start", sm: "center" }}
          spacing={1}
        >
          <Rating value={review.rating} max={5} readOnly />
          <StatusChip label={t("review.card.rating", { rating: review.rating })} tone="info" />
        </Stack>
        {review.comment ? (
          <Typography variant="body1">{review.comment}</Typography>
        ) : (
          <Typography variant="body2" color="text.secondary">
            {t("review.card.noComment")}
          </Typography>
        )}
        <Typography variant="caption" color="text.secondary">
          {t("review.card.target")}: {targetLabel}
        </Typography>
      </Stack>
    </AppCard>
  );
}
