import { Stack, Typography } from "@mui/material";
import { formatDateTime } from "../../../customer/proposals/model/proposalDisplay";
import {
  executionStatusLabel,
  formatExecutionSchedule,
} from "../../../customer/deals/model/dealDisplay";
import { t } from "../../../../shared/i18n";
import { AppCard } from "../../../../shared/ui/AppCard";
import { StatusChip } from "../../../../shared/ui/StatusChip";
import type { ProviderExecution } from "../api/providerExecutionTypes";

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
      <Stack spacing={1}>
        <StatusChip label={executionStatusLabel(execution.status)} />
        <Typography variant="subtitle1">{t("deal.execution.id", { id: execution.id })}</Typography>
        <Typography variant="body2">{t("provider.executions.deal", { id: execution.dealId })}</Typography>
        {execution.businessId != null ? (
          <Typography variant="body2">
            {t("deal.execution.business", { id: execution.businessId })}
          </Typography>
        ) : null}
        <Typography variant="body2" color="text.secondary">
          {schedule
            ? t("deal.execution.scheduled", { value: schedule })
            : t("provider.executions.noSchedule")}
        </Typography>
        {execution.startedAt ? (
          <Typography variant="caption" color="text.secondary">
            {t("deal.execution.started", { date: formatDateTime(execution.startedAt) })}
          </Typography>
        ) : null}
        {execution.completedAt ? (
          <Typography variant="caption" color="text.secondary">
            {t("deal.execution.completed", { date: formatDateTime(execution.completedAt) })}
          </Typography>
        ) : null}
      </Stack>
    </AppCard>
  );
}
