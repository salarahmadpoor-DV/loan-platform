import { Box, CardActionArea, Stack, Typography } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";
import type { HomeCategoryView } from "../../../../shared/mocks/homeMocks";

type CategoryCardProps = {
  category: HomeCategoryView;
  onSelect: (category: HomeCategoryView) => void;
};

export function CategoryCard({ category, onSelect }: CategoryCardProps) {
  const initial = category.title.trim().charAt(0) || "•";

  return (
    <AppCard>
      <CardActionArea
        onClick={() => onSelect(category)}
        sx={{
          display: "block",
          borderRadius: 1,
          mx: -1,
          px: 1,
          py: 0.5,
        }}
      >
        <Stack spacing={1.5} alignItems="flex-start">
          <Box
            aria-hidden
            sx={{
              width: 48,
              height: 48,
              borderRadius: 1,
              bgcolor: "primary.main",
              color: "primary.contrastText",
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
