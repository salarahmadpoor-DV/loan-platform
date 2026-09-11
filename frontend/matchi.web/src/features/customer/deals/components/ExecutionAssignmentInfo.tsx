import { Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import { formatDateTime } from "../../proposals/model/proposalDisplay";
import type { ExecutionAssignment } from "../api/dealTypes";
import { assignmentStatusLabel } from "../model/dealDisplay";

type ExecutionAssignmentInfoProps = {
  assignment: ExecutionAssignment;
};

export function ExecutionAssignmentInfo({ assignment }: ExecutionAssignmentInfoProps) {
  return (
    <Stack spacing={0.5}>
      <Typography variant="body2">
        {t("deal.assignment.provider", { id: assignment.providerId })}
      </Typography>
      <Typography variant="body2">
        {t("deal.assignment.role")}: {assignment.role}
      </Typography>
      <StatusChip
        label={
          assignment.isPrimary ? t("deal.assignment.primary") : t("deal.assignment.notPrimary")
        }
      />
      <StatusChip label={assignmentStatusLabel(assignment.status)} />
      <Typography variant="caption" color="text.secondary">
        {t("deal.assignment.assignedAt", { date: formatDateTime(assignment.assignedAt) })}
      </Typography>
    </Stack>
  );
}
