import { Box } from "@mui/material";
import type { ReactNode } from "react";

type ResponsiveCardGridProps = {
  children: ReactNode;
};

/** 1 column on phone, 2 from tablet up. */
export function ResponsiveCardGrid({ children }: ResponsiveCardGridProps) {
  return (
    <Box
      sx={{
        display: "grid",
        gap: 2,
        gridTemplateColumns: { xs: "1fr", sm: "1fr 1fr" },
        width: "100%",
        minWidth: 0,
      }}
    >
      {children}
    </Box>
  );
}
