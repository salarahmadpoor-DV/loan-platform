import { Box } from "@mui/material";
import type { HomeCategoryView } from "../../../../shared/mocks/homeMocks";
import { CategoryCard } from "./CategoryCard";

type CategoryGridProps = {
  categories: HomeCategoryView[];
  onSelect: (category: HomeCategoryView) => void;
};

export function CategoryGrid({ categories, onSelect }: CategoryGridProps) {
  return (
    <Box
      sx={{
        display: "grid",
        gap: 2,
        gridTemplateColumns: {
          xs: "1fr",
          sm: "repeat(2, minmax(0, 1fr))",
          md: "repeat(3, minmax(0, 1fr))",
          lg: "repeat(5, minmax(0, 1fr))",
        },
      }}
    >
      {categories.map((category) => (
        <Box key={category.id} sx={{ minWidth: 0 }}>
          <CategoryCard category={category} onSelect={onSelect} />
        </Box>
      ))}
    </Box>
  );
}
