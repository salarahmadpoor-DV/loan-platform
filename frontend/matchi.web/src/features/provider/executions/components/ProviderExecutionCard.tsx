import { Stack, Typography } from "@mui/material";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import { formatExecutionSchedule } from "../../../customer/deals/model/dealDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip, type StatusTone } from "../../../../shared/ui/StatusChip";
import type { ProviderExecution } from "../api/providerExecutionTypes";
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
      </Stack>
    </AppCard>
  );
}
