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
        gap: 2,
        py: 4,
      }}
    >
      <CircularProgress size={28} />
      <Typography color="text.secondary">{text}</Typography>
    </Box>
  );
}
