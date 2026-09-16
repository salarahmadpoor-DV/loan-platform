import { Stack, Typography } from "@mui/material";

type RatingStarsProps = {
  value: number;
  reviewCount: number;
  reviewLabel: string;
};

export function RatingStars({ value, reviewCount, reviewLabel }: RatingStarsProps) {
  const rounded = Math.round(value * 10) / 10;
  return (
    <Stack direction="row" spacing={1} alignItems="baseline" flexWrap="wrap" useFlexGap>
      <Typography
        variant="subtitle2"
        component="span"
        aria-label={`${rounded} (${reviewCount})`}
      >
        {rounded.toFixed(1)}
      </Typography>
      <Typography variant="caption" color="text.secondary">
        {reviewLabel}
      </Typography>
    </Stack>
  );
}
