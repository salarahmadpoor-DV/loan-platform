import { Card, CardContent } from "@mui/material";
import type { ReactNode } from "react";

type AppCardProps = {
  children: ReactNode;
};

export function AppCard({ children }: AppCardProps) {
  return (
    <Card variant="outlined">
      <CardContent>{children}</CardContent>
    </Card>
  );
}
