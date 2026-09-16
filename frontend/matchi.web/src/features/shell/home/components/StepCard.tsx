import { Box, Stack, Typography } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";

type StepCardProps = {
  step: number;
  title: string;
  body: string;
  showConnector?: boolean;
};

export function StepCard({ step, title, body, showConnector = false }: StepCardProps) {
  return (
    <Box sx={{ position: "relative", height: "100%" }}>
      {showConnector ? (
        <Box
          aria-hidden
          sx={{
            display: { xs: "none", md: "block" },
            position: "absolute",
            top: 28,
            insetInlineStart: "100%",
            width: 16,
            height: 2,
            bgcolor: "secondary.main",
            zIndex: 1,
          }}
        />
      ) : null}
      <AppCard>
        <Stack spacing={1.25}>
          <Box
            aria-hidden
            sx={{
              width: 36,
              height: 36,
              borderRadius: "50%",
              bgcolor: "secondary.main",
              color: "secondary.contrastText",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              typography: "subtitle2",
            }}
          >
            {step}
          </Box>
          <Typography variant="subtitle1" component="h3">
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {body}
          </Typography>
        </Stack>
      </AppCard>
    </Box>
  );
}
