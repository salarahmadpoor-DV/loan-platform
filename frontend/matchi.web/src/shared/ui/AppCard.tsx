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
        boxShadow: "0 1px 2px rgba(28, 36, 33, 0.04)",
      }}
    >
      <CardContent>{children}</CardContent>
    </Card>
  );
}
