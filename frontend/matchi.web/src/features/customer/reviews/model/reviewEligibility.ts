import type { ServiceExecution } from "../../deals/api/dealTypes";
import { t } from "../../../../shared/i18n";

export type ReviewEligibility = {
  canSubmit: boolean;
  blockers: string[];
  notes: string[];
};

/**
 * Mirrors CreateReviewCommandHandler rules. Product deals do not require execution.
 * Duplicate-target is enforced by the API (400/409), not guessed here.
 */
export function getReviewEligibility(input: {
  dealStatus: string | undefined;
  requestType: string | undefined;
  executions: ServiceExecution[] | undefined;
  executionsLoaded: boolean;
  hasTarget: boolean;
}): ReviewEligibility {
  const blockers: string[] = [];
  const notes: string[] = [];

  if (input.dealStatus && input.dealStatus.toLowerCase() !== "active") {
    blockers.push(t("review.eligibility.dealNotActive"));
  }

  if (input.requestType === "Product") {
    notes.push(t("review.eligibility.productNoExecution"));
  }

  if (input.requestType === "Service" || input.requestType === "Hybrid") {
    notes.push(t("review.eligibility.serviceNeedsExecution"));
    if (input.executionsLoaded) {
      const completed = (input.executions ?? []).some(
        (execution) => execution.status.toLowerCase() === "completed",
      );
      if (!completed) {
        blockers.push(t("review.eligibility.needCompletedExecution"));
      }
    }
  }

  if (input.executionsLoaded && (input.executions?.length ?? 0) === 0) {
    notes.push(t("review.eligibility.noExecutionYet"));
  }

  if (!input.hasTarget) {
    blockers.push(t("review.eligibility.noTarget"));
  }

  notes.push(t("review.eligibility.xor"));
  notes.push(t("review.eligibility.duplicate"));

  const waitingOnData =
    !input.dealStatus ||
    !input.requestType ||
    ((input.requestType === "Service" || input.requestType === "Hybrid") &&
      !input.executionsLoaded);

  return {
    canSubmit: !waitingOnData && blockers.length === 0,
    blockers,
    notes,
  };
}
