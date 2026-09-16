import { Card, CardContent, type SxProps, type Theme } from "@mui/material";
import type { ReactNode } from "react";

type AppCardProps = {
  children: ReactNode;
  sx?: SxProps<Theme>;
};

export function AppCard({ children, sx }: AppCardProps) {
  return (
    <Card
      variant="outlined"
      sx={[
        {
          height: "100%",
          borderColor: "divider",
          bgcolor: "background.paper",
        },
        ...(Array.isArray(sx) ? sx : sx ? [sx] : []),
      ]}
    >
      <CardContent>{children}</CardContent>
    </Card>
  );
}
