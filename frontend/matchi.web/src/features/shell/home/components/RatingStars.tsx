import { Stack, Typography } from "@mui/material";

type RatingStarsProps = {
  value: number;
  reviewCount: number;
  reviewLabel: string;
};

export function RatingStars({ value, reviewCount, reviewLabel }: RatingStarsProps) {
  const rounded = Math.round(value * 10) / 10;
  const filled = Math.max(0, Math.min(5, Math.round(rounded)));
  const stars = "★".repeat(filled) + "☆".repeat(5 - filled);

  return (
    <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
      <Typography
        variant="subtitle2"
        component="span"
        color="primary.main"
        aria-label={`${rounded} (${reviewCount})`}
        sx={{ letterSpacing: "0.06em" }}
      >
        {stars}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        {rounded.toFixed(1)} · {reviewLabel}
      </Typography>
    </Stack>
  );
}
