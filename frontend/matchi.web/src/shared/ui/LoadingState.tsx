import { Box, CircularProgress, Typography } from "@mui/material";
import { t } from "../i18n";

type LoadingStateProps = {
  label?: string;
};

export function LoadingState({ label }: LoadingStateProps) {
  const text = label ?? t("common.loading");
  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: { xs: "center", sm: "flex-start" },
        gap: 2,
        py: { xs: 3, sm: 4 },
        px: 0.5,
      }}
    >
      <CircularProgress size={28} />
      <Typography variant="body2" color="text.secondary">
        {text}
      </Typography>
    </Box>
  );
}
