import { Step, StepButton, Stepper, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";

type MarketplaceStepperProps = {
  steps: string[];
  activeStep: number;
  maxUnlocked: number;
  onStep: (index: number) => void;
};

export function MarketplaceStepper({
  steps,
  activeStep,
  maxUnlocked,
  onStep,
}: MarketplaceStepperProps) {
  const theme = useTheme();
  const isNarrow = useMediaQuery(theme.breakpoints.down("md"));

  return (
    <Stepper
      nonLinear
      activeStep={activeStep}
      orientation={isNarrow ? "vertical" : "horizontal"}
      alternativeLabel={!isNarrow}
      sx={{ mb: { xs: 2, sm: 3 }, width: "100%", minWidth: 0 }}
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
