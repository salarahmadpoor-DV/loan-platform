import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { formatDateTime } from "../../proposals/model/proposalDisplay";
import type { ServiceExecution } from "../api/dealTypes";
import { useExecutionAssignments } from "../hooks/useExecutionAssignments";
import { formatExecutionSchedule } from "../model/dealDisplay";
import { ExecutionAssignmentInfo } from "./ExecutionAssignmentInfo";
import { ExecutionStatusChip } from "./ExecutionStatusChip";

type ExecutionCardProps = {
  execution: ServiceExecution;
};

export function ExecutionCard({ execution }: ExecutionCardProps) {
  const assignmentsQuery = useExecutionAssignments(execution.id);
  const schedule = formatExecutionSchedule([
    execution.scheduledDate,
    execution.scheduledTimeFrom,
    execution.scheduledTimeTo,
  ]);

  return (
    <AppCard>
      <Stack spacing={1.5}>
        <Stack spacing={0.75}>
          <Typography variant="subtitle2">{t("deal.execution.id", { id: execution.id })}</Typography>
          <ExecutionStatusChip status={execution.status} />
          {execution.businessId != null ? (
            <Typography variant="body2">
              {t("deal.execution.business", { id: execution.businessId })}
            </Typography>
          ) : null}
          {schedule ? (
            <Typography variant="body2">
              {t("deal.execution.scheduled", { value: schedule })}
            </Typography>
          ) : null}
          {execution.startedAt ? (
            <Typography variant="body2">
              {t("deal.execution.started", { date: formatDateTime(execution.startedAt) })}
            </Typography>
          ) : null}
          {execution.completedAt ? (
            <Typography variant="body2">
              {t("deal.execution.completed", { date: formatDateTime(execution.completedAt) })}
            </Typography>
          ) : null}
          <Typography variant="caption" color="text.secondary">
            {t("deal.execution.created", { date: formatDateTime(execution.createDate) })}
          </Typography>
        </Stack>

        <Typography variant="subtitle2">{t("deal.assignment.title")}</Typography>
        {assignmentsQuery.isPending ? (
          <LoadingState label={t("deal.assignment.loading")} />
        ) : null}
        {assignmentsQuery.isError ? (
          <Stack spacing={1}>
            <ErrorAlert error={assignmentsQuery.error} />
            <Button
              variant="outlined"
              size="small"
              onClick={() => {
                void assignmentsQuery.refetch();
              }}
              disabled={assignmentsQuery.isFetching}
              sx={{ alignSelf: "flex-start" }}
            >
              {t("deal.assignment.retry")}
            </Button>
          </Stack>
        ) : null}
        {!assignmentsQuery.isPending &&
        !assignmentsQuery.isError &&
        (assignmentsQuery.data?.length ?? 0) === 0 ? (
          <EmptyState title={t("deal.assignment.empty")} body={t("deal.assignment.emptyBody")} />
        ) : null}
        {assignmentsQuery.data && assignmentsQuery.data.length > 0 ? (
          <Stack spacing={1.5}>
            {assignmentsQuery.data.map((assignment) => (
              <ExecutionAssignmentInfo key={assignment.id} assignment={assignment} />
            ))}
          </Stack>
        ) : null}
      </Stack>
    </AppCard>
  );
}
