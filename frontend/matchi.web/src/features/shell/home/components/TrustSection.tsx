import { Box, Stack, Typography } from "@mui/material";
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
        gridTemplateColumns: { xs: "1fr", sm: "repeat(2, minmax(0, 1fr))", lg: "repeat(3, minmax(0, 1fr))" },
      }}
    >
      {items.map((item, index) => (
        <AppCard key={item.title}>
          <Stack spacing={1}>
            <Box
              aria-hidden
              sx={{
                width: 32,
                height: 32,
                borderRadius: 1,
                bgcolor: "primary.main",
                color: "primary.contrastText",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                typography: "caption",
                fontWeight: 700,
              }}
            >
              {index + 1}
            </Box>
            <Typography variant="subtitle1" component="h3">
              {item.title}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {item.body}
            </Typography>
          </Stack>
        </AppCard>
      ))}
    </Box>
  );
}
