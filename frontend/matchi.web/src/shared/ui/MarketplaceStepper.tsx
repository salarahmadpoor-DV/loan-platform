import { Box, LinearProgress, Stack, Step, StepButton, Stepper, Typography, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";

type MarketplaceStepperProps = {
  steps: string[];
  activeStep: number;
  maxUnlocked: number;
  onStep: (index: number) => void;
  progressLabel?: string;
};

export function MarketplaceStepper({
  steps,
  activeStep,
  maxUnlocked,
  onStep,
  progressLabel,
}: MarketplaceStepperProps) {
  const theme = useTheme();
  const isNarrow = useMediaQuery(theme.breakpoints.down("md"));
  const progress = ((activeStep + 1) / steps.length) * 100;

  if (isNarrow) {
    return (
      <Stack spacing={1} sx={{ mb: 2, width: "100%", minWidth: 0 }}>
        {progressLabel ? (
          <Typography variant="caption" color="text.secondary">
            {progressLabel}
          </Typography>
        ) : null}
        <Typography variant="subtitle1" fontWeight={700}>
          {steps[activeStep]}
        </Typography>
        <LinearProgress
          variant="determinate"
          value={progress}
          aria-hidden
          sx={{ height: 6, borderRadius: 999, bgcolor: "action.hover" }}
        />
        <Box sx={{ display: "flex", flexWrap: "wrap", gap: 0.75, pt: 0.5 }}>
          {steps.map((label, index) => (
            <Box
              key={label}
              component="button"
              type="button"
              disabled={index > maxUnlocked}
              onClick={() => onStep(index)}
              aria-current={index === activeStep ? "step" : undefined}
              aria-label={label}
              sx={{
                width: 10,
                height: 10,
                p: 0,
                border: 0,
                borderRadius: "50%",
                cursor: index > maxUnlocked ? "default" : "pointer",
                bgcolor:
                  index === activeStep
                    ? "primary.main"
                    : index < activeStep
                      ? "secondary.main"
                      : "action.disabledBackground",
              }}
            />
          ))}
        </Box>
      </Stack>
    );
  }

  return (
    <Stepper
      nonLinear
      activeStep={activeStep}
      orientation="horizontal"
      alternativeLabel
      sx={{ mb: 3, width: "100%", minWidth: 0 }}
    >
      {steps.map((label, index) => (
        <Step key={label} completed={index < activeStep}>
          <StepButton
            color="inherit"
            onClick={() => onStep(index)}
            disabled={index > maxUnlocked}
            aria-current={index === activeStep ? "step" : undefined}
          >
            {label}
          </StepButton>
        </Step>
      ))}
    </Stepper>
  );
}
