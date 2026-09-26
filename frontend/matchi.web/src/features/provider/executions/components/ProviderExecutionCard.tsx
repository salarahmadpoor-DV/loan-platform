import { Button, Stack, Typography } from "@mui/material";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { formatExecutionSchedule } from "../../../customer/deals/model/dealDisplay";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip, type StatusTone } from "../../../../shared/ui/StatusChip";
import type { ProviderExecution } from "../api/providerExecutionTypes";
import { useCompleteProviderExecution, useStartProviderExecution } from "../hooks/useProviderExecutionActions";
import { providerExecutionStatusLabel } from "../model/providerExecutionDisplay";

function executionTone(status: string): StatusTone {
  const key = status.toLowerCase();
  if (key === "pending") {
    return "pending";
  }
  if (key === "inprogress") {
    return "info";
  }
  if (key === "completed") {
    return "success";
  }
  return "neutral";
}

type ProviderExecutionCardProps = {
  execution: ProviderExecution;
};

export function ProviderExecutionCard({ execution }: ProviderExecutionCardProps) {
  const schedule = formatExecutionSchedule([
    execution.scheduledDate,
    execution.scheduledTimeFrom,
    execution.scheduledTimeTo,
  ]);
  const status = execution.status.toLowerCase();
  const start = useStartProviderExecution(execution.dealId);
  const complete = useCompleteProviderExecution(execution.dealId);
  const pending = start.isPending || complete.isPending;
  const actionError = start.error ?? complete.error;

  return (
    <AppCard>
      <Stack spacing={1.5} sx={{ height: "100%" }}>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="flex-start"
          spacing={1}
          sx={{ flexWrap: "wrap" }}
          useFlexGap
        >
          <Typography variant="h6">{t("deal.execution.id", { id: execution.id })}</Typography>
          <StatusChip
            label={providerExecutionStatusLabel(execution.status)}
            tone={executionTone(execution.status)}
          />
        </Stack>
        <Typography variant="body2">{t("provider.executions.deal", { id: execution.dealId })}</Typography>
        {execution.businessId != null ? (
          <Typography variant="body2">
            {t("deal.execution.business", { id: execution.businessId })}
          </Typography>
        ) : null}
        <Typography variant="body2">
          {schedule
            ? t("deal.execution.scheduled", { value: schedule })
            : t("provider.executions.noSchedule")}
        </Typography>
        <Typography variant="body2">
          {t("deal.execution.started", {
            date: execution.startedAt
              ? formatDateTime(execution.startedAt)
              : t("common.notSpecified"),
          })}
        </Typography>
        <Typography variant="body2">
          {t("deal.execution.completed", {
            date: execution.completedAt
              ? formatDateTime(execution.completedAt)
              : t("common.notSpecified"),
          })}
        </Typography>
        {actionError ? <ErrorAlert error={actionError} /> : null}
        {status === "pending" ? (
          <Button
            variant="contained"
            disabled={pending}
            onClick={() => {
              start.reset();
              complete.reset();
              start.mutate(execution.id);
            }}
            sx={{ alignSelf: "flex-start", minHeight: 44 }}
          >
            {start.isPending ? t("provider.executions.starting") : t("provider.executions.start")}
          </Button>
        ) : null}
        {status === "inprogress" ? (
          <Button
            variant="contained"
            disabled={pending}
            onClick={() => {
              start.reset();
              complete.reset();
              complete.mutate(execution.id);
            }}
            sx={{ alignSelf: "flex-start", minHeight: 44 }}
          >
            {complete.isPending ? t("provider.executions.completing") : t("provider.executions.complete")}
          </Button>
        ) : null}
      </Stack>
    </AppCard>
  );
}
