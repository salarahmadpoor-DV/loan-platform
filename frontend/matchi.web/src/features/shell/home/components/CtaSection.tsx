import { Box, Button, Stack, Typography } from "@mui/material";
import { matchiRadius, matchiShadows } from "../../../../app/designTokens";

type CtaSectionProps = {
  title: string;
  body: string;
  primaryLabel: string;
  secondaryLabel: string;
  onPrimary: () => void;
  onSecondary: () => void;
};

export function CtaSection({
  title,
  body,
  primaryLabel,
  secondaryLabel,
  onPrimary,
  onSecondary,
}: CtaSectionProps) {
  return (
    <Box
      sx={{
        p: { xs: 3, md: 5 },
        borderRadius: matchiRadius.lg,
        bgcolor: "primary.main",
        color: "primary.contrastText",
        boxShadow: matchiShadows.elevated,
      }}
    >
      <Typography variant="h2" component="h2" color="inherit">
        {title}
      </Typography>
      <Typography variant="body1" sx={{ mt: 1.5, mb: 3, opacity: 0.92, maxWidth: 560 }}>
        {body}
      </Typography>
      <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5}>
        <Button variant="contained" color="secondary" size="large" onClick={onPrimary}>
          {primaryLabel}
        </Button>
        <Button
          variant="outlined"
          size="large"
          onClick={onSecondary}
          sx={{
            color: "primary.contrastText",
            borderColor: "primary.contrastText",
            "&:hover": {
              borderColor: "primary.contrastText",
              bgcolor: "action.hover",
            },
          }}
        >
          {secondaryLabel}
        </Button>
      </Stack>
    </Box>
  );
}
