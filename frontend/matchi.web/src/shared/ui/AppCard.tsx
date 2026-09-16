import { Card, CardContent } from "@mui/material";
import type { ReactNode } from "react";

type AppCardProps = {
  children: ReactNode;
};

export function AppCard({ children }: AppCardProps) {
  return (
    <Card
      variant="outlined"
      sx={{
        height: "100%",
        borderColor: "divider",
        bgcolor: "background.paper",
      }}
    >
      <CardContent>{children}</CardContent>
    </Card>
  );
}
