import { Box, Stack, Typography } from "@mui/material";
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
    <Box
      sx={{
        p: 1.5,
        borderRadius: 1,
        border: 1,
        borderColor: assignment.isPrimary ? "primary.main" : "divider",
        bgcolor: assignment.isPrimary ? "action.hover" : "transparent",
      }}
    >
      <Stack spacing={0.75}>
        <Stack direction="row" spacing={1} sx={{ flexWrap: "wrap" }} useFlexGap>
          {assignment.isPrimary ? (
            <StatusChip label={t("deal.assignment.primary")} tone="primary" />
          ) : (
            <StatusChip label={t("deal.assignment.notPrimary")} />
          )}
          <StatusChip
            label={assignmentStatusLabel(assignment.status)}
            tone={assignment.status.toLowerCase() === "assigned" ? "info" : "neutral"}
          />
        </Stack>
        <Typography variant="subtitle2">
          {t("deal.assignment.provider", { id: assignment.providerId })}
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {t("deal.assignment.role")}: {assignment.role}
        </Typography>
        <Typography variant="caption" color="text.secondary">
          {t("deal.assignment.assignedAt", { date: formatDateTime(assignment.assignedAt) })}
        </Typography>
      </Stack>
    </Box>
  );
}
