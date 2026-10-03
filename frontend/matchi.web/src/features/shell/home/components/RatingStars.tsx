import Star from "@mui/icons-material/Star";
import StarBorder from "@mui/icons-material/StarBorder";
import { Stack, Typography } from "@mui/material";

type RatingStarsProps = {
  value: number;
  reviewCount: number;
  reviewLabel: string;
};

export function RatingStars({ value, reviewCount, reviewLabel }: RatingStarsProps) {
  const rounded = Math.round(value * 10) / 10;
  const filled = Math.max(0, Math.min(5, Math.round(rounded)));

  return (
    <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap" useFlexGap>
      <Stack
        direction="row"
        spacing={0.15}
        component="span"
        aria-label={`${rounded} (${reviewCount})`}
        sx={{ color: "primary.main" }}
      >
        {Array.from({ length: 5 }, (_, index) =>
          index < filled ? (
            <Star key={index} sx={{ fontSize: 18 }} aria-hidden />
          ) : (
            <StarBorder key={index} sx={{ fontSize: 18 }} aria-hidden />
          ),
        )}
      </Stack>
      <Typography variant="caption" color="text.secondary">
        {rounded.toFixed(1)} · {reviewLabel}
      </Typography>
    </Stack>
  );
}
