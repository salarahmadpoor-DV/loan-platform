import { Box, Typography } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";

type TrustItem = {
  title: string;
  body: string;
};

type TrustSectionProps = {
  items: TrustItem[];
};

export function TrustSection({ items }: TrustSectionProps) {
  return (
    <Box
      sx={{
        display: "grid",
        gap: 2,
        gridTemplateColumns: { xs: "1fr", sm: "repeat(2, minmax(0, 1fr))", md: "repeat(3, minmax(0, 1fr))" },
      }}
    >
      {items.map((item) => (
        <AppCard key={item.title}>
          <Typography variant="h3" component="h3" sx={{ mb: 1 }}>
            {item.title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {item.body}
          </Typography>
        </AppCard>
      ))}
    </Box>
  );
}
