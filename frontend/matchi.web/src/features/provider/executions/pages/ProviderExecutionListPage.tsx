import { Button, Stack, Typography } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { groupByStatus } from "../../../../shared/marketplace/groupByStatus";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ResponsiveCardGrid } from "../../../../shared/ui/ResponsiveCardGrid";
import { ProviderExecutionCard } from "../components/ProviderExecutionCard";
import { useMyProviderExecutions } from "../hooks/useMyProviderExecutions";
import { providerExecutionStatusLabel } from "../model/providerExecutionDisplay";

export function ProviderExecutionListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderExecutions();
  const groups = groupByStatus(data ?? [], (item) => item.status, [
    "Pending",
    "InProgress",
    "Completed",
    "Cancelled",
  ]);

  return (
    <>
      <PageHeader
        title={t("provider.executions.title")}
        description={t("provider.executions.description")}
      />
      {isPending ? <LoadingState label={t("provider.executions.loading")} /> : null}
      {isError ? (
        <Stack spacing={2}>
          <ErrorAlert error={error} />
          <Button
            variant="outlined"
            onClick={() => {
              void refetch();
            }}
            disabled={isFetching}
            sx={{ minHeight: 44, alignSelf: "flex-start" }}
          >
            {t("provider.executions.retry")}
          </Button>
        </Stack>
      ) : null}
      {!isPending && !isError && data?.length === 0 ? (
        <EmptyState
          title={t("provider.executions.emptyTitle")}
          body={t("provider.executions.emptyBody")}
        />
      ) : null}
      {groups.map((group) => (
        <Stack key={group.status} spacing={1.5} sx={{ mb: 3 }}>
          <Typography variant="h6">{providerExecutionStatusLabel(group.status)}</Typography>
          <ResponsiveCardGrid>
            {group.items.map((execution) => (
              <ProviderExecutionCard key={execution.id} execution={execution} />
            ))}
          </ResponsiveCardGrid>
        </Stack>
      ))}
    </>
  );
}
