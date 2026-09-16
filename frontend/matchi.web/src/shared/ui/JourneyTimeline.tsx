import { Step, StepButton, Stepper, Typography, useMediaQuery } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useNavigate } from "react-router-dom";
import { t, type MessageKey } from "../i18n";
import type { JourneyStepId, JourneyStepView } from "../marketplace/customerJourney";

const STEP_LABEL: Record<JourneyStepId, MessageKey> = {
  request: "journey.request",
  matching: "journey.matching",
  proposal: "journey.proposal",
  deal: "journey.deal",
  execution: "journey.execution",
  review: "journey.review",
};

const STATE_LABEL: Record<JourneyStepView["state"], MessageKey> = {
  complete: "journey.complete",
  current: "journey.current",
  upcoming: "journey.upcoming",
  unknown: "journey.unknown",
  notApplicable: "journey.notApplicable",
};

type JourneyTimelineProps = {
  steps: JourneyStepView[];
};

export function JourneyTimeline({ steps }: JourneyTimelineProps) {
  const theme = useTheme();
  const isNarrow = useMediaQuery(theme.breakpoints.down("md"));
  const navigate = useNavigate();
  const currentIndex = Math.max(
    0,
    steps.findIndex((step) => step.state === "current"),
  );

  return (
    <Stepper
      activeStep={currentIndex}
      orientation={isNarrow ? "vertical" : "horizontal"}
      alternativeLabel={!isNarrow}
      sx={{ width: "100%", minWidth: 0, mb: 2 }}
      aria-label={t("journey.title")}
    >
      {steps.map((step) => {
        const clickable = Boolean(step.href) && step.state !== "notApplicable";
        return (
          <Step
            key={step.id}
            completed={step.state === "complete"}
            disabled={step.state === "notApplicable"}
          >
            <StepButton
              color="inherit"
              optional={
                <Typography variant="caption" color="text.secondary">
                  {t(STATE_LABEL[step.state])}
                </Typography>
              }
              onClick={() => {
                if (clickable && step.href) {
                  navigate(step.href);
                }
              }}
              disabled={!clickable}
            >
              {t(STEP_LABEL[step.id])}
            </StepButton>
          </Step>
        );
      })}
    </Stepper>
  );
}
