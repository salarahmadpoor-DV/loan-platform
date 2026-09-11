import { Box, Card, CardContent, Typography } from "@mui/material";
import type { ReactNode } from "react";

type EmptyStateProps = {
  title: string;
  body?: string;
  action?: ReactNode;
};

export function EmptyState({ title, body, action }: EmptyStateProps) {
  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="subtitle1">{title}</Typography>
        {body ? (
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            {body}
          </Typography>
        ) : null}
        {action ? <Box sx={{ mt: 2 }}>{action}</Box> : null}
      </CardContent>
    </Card>
  );
}
