import { Box } from "@mui/material";
import type { ReactNode } from "react";

type PageContainerProps = {
  children: ReactNode;
};

/** Caps content width on laptop/desktop/wide screens; full width on phone/tablet. */
export function PageContainer({ children }: PageContainerProps) {
  return (
    <Box
      sx={{
        width: "100%",
        maxWidth: { xs: "100%", lg: 1120, xl: 1280 },
        mx: "auto",
        minWidth: 0,
      }}
    >
      {children}
    </Box>
  );
}
