import { Box, Stack, Typography } from "@mui/material";
import { AppCard } from "../../../../shared/ui/AppCard";

type StepCardProps = {
  step: number;
  title: string;
  body: string;
};

export function StepCard({ step, title, body }: StepCardProps) {
  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Box
          aria-hidden
          sx={{
            width: 40,
            height: 40,
            borderRadius: "50%",
            bgcolor: "secondary.main",
            color: "secondary.contrastText",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            typography: "subtitle1",
          }}
        >
          {step}
        </Box>
        <Typography variant="h3" component="h3">
          {title}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {body}
        </Typography>
      </Stack>
    </AppCard>
  );
}
