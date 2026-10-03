import { Box, Typography } from "@mui/material";
import type { ReactNode } from "react";

type SectionHeaderProps = {
  title: string;
  subtitle?: string;
  action?: ReactNode;
  id?: string;
  compact?: boolean;
};

export function SectionHeader({ title, subtitle, action, id, compact = false }: SectionHeaderProps) {
  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: { xs: "column", sm: "row" },
        alignItems: { sm: compact ? "center" : "flex-end" },
        justifyContent: "space-between",
        gap: compact ? 1 : 2,
        mb: compact ? 1.25 : 3,
      }}
    >
      <Box sx={{ minWidth: 0 }}>
        <Typography variant={compact ? "subtitle1" : "h2"} component="h2" id={id}>
          {title}
        </Typography>
        {subtitle ? (
          <Typography
            variant={compact ? "body2" : "body1"}
            color="text.secondary"
            sx={{ mt: compact ? 0.25 : 1, maxWidth: 640 }}
          >
            {subtitle}
          </Typography>
        ) : null}
      </Box>
      {action}
    </Box>
  );
}
