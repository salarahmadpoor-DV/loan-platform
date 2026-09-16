import { Box } from "@mui/material";
import type { ReactNode } from "react";

type FormSplitLayoutProps = {
  main: ReactNode;
  summary: ReactNode;
  wide?: boolean;
};

/** Desktop: form + sticky summary. Mobile: stacked, no overflow. */
export function FormSplitLayout({ main, summary, wide = false }: FormSplitLayoutProps) {
  return (
    <Box
      sx={{
        display: "grid",
        gap: { xs: 2, md: 3 },
        alignItems: "start",
        gridTemplateColumns: {
          xs: "1fr",
          md: wide ? "minmax(0, 1fr) minmax(260px, 360px)" : "minmax(0, 1fr) minmax(240px, 320px)",
        },
        maxWidth: { md: wide ? "100%" : 1080 },
        width: "100%",
        minWidth: 0,
      }}
    >
      <Box sx={{ minWidth: 0 }}>{main}</Box>
      <Box
        sx={{
          minWidth: 0,
          position: { md: "sticky" },
          top: { md: 80 },
        }}
      >
        {summary}
      </Box>
    </Box>
  );
}
