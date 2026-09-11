import { Alert, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import type { ReviewEligibility } from "../model/reviewEligibility";

type ReviewEligibilityInfoProps = {
  eligibility: ReviewEligibility;
};

export function ReviewEligibilityInfo({ eligibility }: ReviewEligibilityInfoProps) {
  return (
    <Stack spacing={1}>
      <Typography variant="subtitle2">{t("review.eligibility.title")}</Typography>
      {eligibility.canSubmit ? (
        <Alert severity="success">{t("review.eligibility.ready")}</Alert>
      ) : (
        <Alert severity="info">{t("review.eligibility.blocked")}</Alert>
      )}
      {eligibility.blockers.map((reason) => (
        <Typography key={reason} variant="body2">
          {reason}
        </Typography>
      ))}
      {eligibility.notes.map((note) => (
        <Typography key={note} variant="body2" color="text.secondary">
          {note}
        </Typography>
      ))}
    </Stack>
  );
}
