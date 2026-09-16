import { Box, CardActionArea, Stack, Typography } from "@mui/material";
import { matchiShadows } from "../../../../app/designTokens";
import type { HomeCategoryView } from "../../../../shared/mocks/homeMocks";
import { AppCard } from "../../../../shared/ui/AppCard";

type CategoryCardProps = {
  category: HomeCategoryView;
  onSelect: (category: HomeCategoryView) => void;
};

export function CategoryCard({ category, onSelect }: CategoryCardProps) {
  const initial = category.title.trim().charAt(0) || "•";

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
              borderRadius: 1,
              bgcolor: "secondary.light",
              color: "secondary.dark",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              typography: "subtitle1",
            }}
          >
            {initial}
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
