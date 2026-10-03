import { Box, CardActionArea, Stack, Typography } from "@mui/material";
import { matchiShadows } from "../../../../app/designTokens";
import type { HomeCategoryView } from "../../../../shared/mocks/homeMocks";
import { AppCard } from "../../../../shared/ui/AppCard";
import { categoryGlyph } from "./categoryGlyph";

type CategoryCardProps = {
  category: HomeCategoryView;
  onSelect: (category: HomeCategoryView) => void;
};

const ICON_TONES = [
  { bgcolor: "rgba(37, 99, 235, 0.10)", color: "primary.dark" },
  { bgcolor: "rgba(13, 148, 136, 0.12)", color: "success.dark" },
  { bgcolor: "rgba(217, 119, 6, 0.12)", color: "warning.dark" },
  { bgcolor: "rgba(2, 132, 199, 0.12)", color: "info.dark" },
] as const;

function iconTone(id: string) {
  let hash = 0;
  for (const char of id) {
    hash = (hash + char.charCodeAt(0)) % ICON_TONES.length;
  }
  return ICON_TONES[hash] ?? ICON_TONES[0];
}

export function CategoryCard({ category, onSelect }: CategoryCardProps) {
  const Glyph = categoryGlyph(category.id, category.title);
  const tone = iconTone(category.id);

  return (
    <AppCard
      sx={{
        transition: "border-color 0.15s ease, box-shadow 0.15s ease",
        "&:hover": {
          borderColor: "primary.main",
          boxShadow: matchiShadows.hover,
        },
        "&:focus-within": {
          borderColor: "primary.main",
        },
      }}
    >
      <CardActionArea
        onClick={() => onSelect(category)}
        sx={{
          display: "block",
          borderRadius: 1,
          mx: -1,
          px: 1,
          py: 0.5,
          "&:focus-visible": {
            outline: "2px solid",
            outlineColor: "primary.main",
            outlineOffset: 2,
          },
        }}
      >
        <Stack spacing={1.25} alignItems="flex-start">
          <Box
            aria-hidden
            sx={{
              width: 44,
              height: 44,
              borderRadius: 1.5,
              bgcolor: tone.bgcolor,
              color: tone.color,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <Glyph sx={{ fontSize: 22 }} aria-hidden />
          </Box>
          <Typography variant="subtitle1" component="h3">
            {category.title}
          </Typography>
          {category.description ? (
            <Typography variant="body2" color="text.secondary">
              {category.description}
            </Typography>
          ) : null}
          {category.count != null ? (
            <Typography variant="caption" color="text.secondary">
              {category.count}
            </Typography>
          ) : null}
        </Stack>
      </CardActionArea>
    </AppCard>
  );
}
