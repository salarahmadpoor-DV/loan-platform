import { Button, Stack } from "@mui/material";
import { t } from "../../../../shared/i18n";
import { EmptyState } from "../../../../shared/ui/EmptyState";
import { ErrorAlert } from "../../../../shared/ui/ErrorAlert";
import { LoadingState } from "../../../../shared/ui/LoadingState";
import { PageHeader } from "../../../../shared/ui/PageHeader";
import { ProviderExecutionCard } from "../components/ProviderExecutionCard";
import { useMyProviderExecutions } from "../hooks/useMyProviderExecutions";

export function ProviderExecutionListPage() {
  const { data, isPending, isError, error, refetch, isFetching } = useMyProviderExecutions();

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
            sx={{ alignSelf: "flex-start" }}
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
      {data && data.length > 0 ? (
        <Stack spacing={2}>
          {data.map((execution) => (
            <ProviderExecutionCard key={execution.id} execution={execution} />
          ))}
        </Stack>
      ) : null}
    </>
  );
}
